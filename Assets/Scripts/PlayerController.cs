using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float crouchSpeed = 1.5f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    public float standingHeight = 1.8f;
    public float crouchTransitionSpeed = 10f;
    private bool isCrouching = false;

    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f;
    public float lookXLimit = 80.0f;

    [Header("Headbob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    private float defaultCameraY;
    private float targetCameraY;
    private float currentBaseY;
    private float timer;

    [Header("Lean Settings")]
    public float leanDistance = 0.18f;
    public float leanAngle = 10f;
    public float leanSpeed = 6f;
    public float headRadius = 0.15f;
    public float wallPadding = 0.04f;
    private float currentLeanX;
    private float currentLeanRotZ;

    [Header("References")]
    public Camera playerCamera;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultCameraY = playerCamera.transform.localPosition.y;
        targetCameraY = defaultCameraY;
        currentBaseY = defaultCameraY;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canMove || SettingsManager.Instance == null) return;

        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        HandleLean();
        HandleHeadbob();
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentLeanRotZ);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
    }

    private void HandleMovement()
    {
        float inputY = (SettingsManager.Instance.GetKey(SettingsManager.MoveFwd) ? 1f : 0f) -
                       (SettingsManager.Instance.GetKey(SettingsManager.MoveBck) ? 1f : 0f);

        float inputX = (SettingsManager.Instance.GetKey(SettingsManager.MoveRgt) ? 1f : 0f) -
                       (SettingsManager.Instance.GetKey(SettingsManager.MoveLft) ? 1f : 0f);

        Vector2 inputDir = new Vector2(inputX, inputY).normalized;
        float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;

        float verticalVelocity = characterController.isGrounded ? -0.5f : moveDirection.y + gravity * Time.deltaTime;

        moveDirection = (transform.forward * (inputDir.y * currentSpeed)) + (transform.right * (inputDir.x * currentSpeed));
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (SettingsManager.Instance.GetKeyDown(SettingsManager.Crouch))
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        characterController.center = Vector3.down * (standingHeight - characterController.height) / 2.0f;

        targetCameraY = defaultCameraY - (standingHeight - targetHeight);
    }

    private void HandleLean()
    {
        float leanInput = (SettingsManager.Instance.GetKey(SettingsManager.LeanRgt) ? 1f : 0f) -
                          (SettingsManager.Instance.GetKey(SettingsManager.LeanLft) ? 1f : 0f);

        float targetLeanX = leanInput * leanDistance;
        float targetLeanRotZ = leanInput * -leanAngle;

        if (leanInput != 0)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * targetCameraY;
            Vector3 rayDirection = transform.right * leanInput;

            if (Physics.SphereCast(rayOrigin, headRadius, rayDirection, out RaycastHit hit, leanDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                float allowedDistance = Mathf.Max(0, hit.distance - wallPadding);
                float hitRatio = allowedDistance / leanDistance;

                targetLeanX = leanInput * allowedDistance;
                targetLeanRotZ = leanInput * (-leanAngle * hitRatio);
            }
        }

        currentLeanX = Mathf.Lerp(currentLeanX, targetLeanX, Time.deltaTime * leanSpeed);
        currentLeanRotZ = Mathf.Lerp(currentLeanRotZ, targetLeanRotZ, Time.deltaTime * leanSpeed);
    }

    private void HandleHeadbob()
    {
        if (!characterController.isGrounded) return;

        float currentBobY = 0f;
        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? bobSpeed * 0.5f : bobSpeed);
            currentBobY = Mathf.Sin(timer) * bobAmount;
        }
        else
        {
            timer = 0;
        }

        currentBaseY = Mathf.Lerp(currentBaseY, targetCameraY + currentBobY, Time.deltaTime * crouchTransitionSpeed);
        float leanDip = Mathf.Abs(currentLeanX) * 0.1f;

        playerCamera.transform.localPosition = new Vector3(currentLeanX, currentBaseY - leanDip, playerCamera.transform.localPosition.z);
    }
}
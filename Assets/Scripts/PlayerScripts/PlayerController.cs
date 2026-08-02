using UnityEngine;
using NodeZero.Core;

namespace NodeZero.Interaction
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Настройки перемещения")]
        [SerializeField] private float _walkSpeed = 3.0f;
        [SerializeField] private float _crouchSpeed = 1.5f;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Настройки приседания")]
        [SerializeField] private float _crouchHeight = 1.0f;
        [SerializeField] private float _standingHeight = 1.8f;
        [SerializeField] private float _crouchTransitionSpeed = 10f;
        private bool _isCrouching = false;

        [Header("Настройки обзора")]
        [SerializeField] private float _mouseSensitivity = 0.5f;
        [SerializeField] private float _lookXLimit = 80.0f;

        [Header("Настройки тряски камеры (Headbob)")]
        [SerializeField] private float _bobSpeed = 10f;
        [SerializeField] private float _bobAmount = 0.05f;
        private float _defaultCameraY;
        private float _targetCameraY;
        private float _currentBaseY;
        private float _timer;

        [Header("Настройки наклонов (Lean)")]
        [SerializeField] private float _leanDistance = 0.18f;
        [SerializeField] private float _leanAngle = 10f;
        [SerializeField] private float _leanSpeed = 6f;
        [SerializeField] private float _headRadius = 0.15f;
        [SerializeField] private float _wallPadding = 0.04f;
        private float _currentLeanX;
        private float _currentLeanRotZ;

        [Header("Ссылки")]
        public Camera playerCamera;

        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;
        private float _rotationX = 0;

        public bool canMove = true;

        // Подписываемся на события при включении скрипта
        private void OnEnable()
        {
            EventBus.OnPlayerStateChanged += SetMovementState;
        }

        // Отписываемся при выключении во избежание утечек памяти
        private void OnDisable()
        {
            EventBus.OnPlayerStateChanged -= SetMovementState;
        }

        // Метод, который срабатывает, когда EventBus.RaisePlayerStateChanged вызван
        private void SetMovementState(bool state)
        {
            canMove = state;
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _defaultCameraY = playerCamera.transform.localPosition.y;
            _targetCameraY = _defaultCameraY;
            _currentBaseY = _defaultCameraY;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (SettingsManager.Instance == null) return;

            if (canMove)
            {
                HandleMouseLook();
                HandleCrouch();
                HandleLean();
                HandleHeadbob();
            }

            // Гравитация и инерция рассчитываются каждый кадр
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            Vector2 lookDelta = SettingsManager.Instance.GetLookDelta();

            _rotationX -= lookDelta.y * _mouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, _currentLeanRotZ);
            transform.rotation *= Quaternion.Euler(0, lookDelta.x * _mouseSensitivity, 0);
        }

        private void HandleMovement()
        {
            // Обнуляем ввод, если открыт инвентарь
            Vector2 inputDir = canMove ? SettingsManager.Instance.GetMoveDelta() : Vector2.zero;

            float currentSpeed = _isCrouching ? _crouchSpeed : _walkSpeed;
            float verticalVelocity = _characterController.isGrounded ? -0.5f : _moveDirection.y + _gravity * Time.deltaTime;

            _moveDirection = (transform.forward * (inputDir.y * currentSpeed)) + (transform.right * (inputDir.x * currentSpeed));
            _moveDirection.y = verticalVelocity;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        private void HandleCrouch()
        {
            if (SettingsManager.Instance.IsCrouchPressed())
                _isCrouching = !_isCrouching;

            float targetHeight = _isCrouching ? _crouchHeight : _standingHeight;
            _characterController.height = Mathf.Lerp(_characterController.height, targetHeight, Time.deltaTime * _crouchTransitionSpeed);
            _characterController.center = Vector3.down * (_standingHeight - _characterController.height) / 2.0f;

            _targetCameraY = _defaultCameraY - (_standingHeight - targetHeight);
        }

        private void HandleLean()
        {
            float leanInput = SettingsManager.Instance.GetLeanDelta();

            float targetLeanX = leanInput * _leanDistance;
            float targetLeanRotZ = leanInput * -_leanAngle;

            if (leanInput != 0)
            {
                Vector3 rayOrigin = transform.position + Vector3.up * _targetCameraY;
                Vector3 rayDirection = transform.right * leanInput;

                if (Physics.SphereCast(rayOrigin, _headRadius, rayDirection, out RaycastHit hit, _leanDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    float allowedDistance = Mathf.Max(0, hit.distance - _wallPadding);
                    float hitRatio = allowedDistance / _leanDistance;

                    targetLeanX = leanInput * allowedDistance;
                    targetLeanRotZ = leanInput * (-_leanAngle * hitRatio);
                }
            }

            _currentLeanX = Mathf.Lerp(_currentLeanX, targetLeanX, Time.deltaTime * _leanSpeed);
            _currentLeanRotZ = Mathf.Lerp(_currentLeanRotZ, targetLeanRotZ, Time.deltaTime * _leanSpeed);
        }

        private void HandleHeadbob()
        {
            if (!_characterController.isGrounded) return;

            float currentBobY = 0f;
            if (Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
            {
                _timer += Time.deltaTime * (_isCrouching ? _bobSpeed * 0.5f : _bobSpeed);
                currentBobY = Mathf.Sin(_timer) * _bobAmount;
            }
            else
            {
                _timer = 0;
            }

            _currentBaseY = Mathf.Lerp(_currentBaseY, _targetCameraY + currentBobY, Time.deltaTime * _crouchTransitionSpeed);
            float leanDip = Mathf.Abs(_currentLeanX) * 0.1f;

            playerCamera.transform.localPosition = new Vector3(_currentLeanX, _currentBaseY - leanDip, playerCamera.transform.localPosition.z);
        }
    }
}
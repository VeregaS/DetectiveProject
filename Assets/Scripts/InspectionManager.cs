using UnityEngine;
using TMPro;

public class InspectionManager : MonoBehaviour
{
    [Header("UI Осмотра")]
    public GameObject inspectionUI;
    public TextMeshProUGUI inspectionText;

    [Header("Настройки камеры")]
    public Transform inspectPoint;
    public PlayerController player;
    public NotebookManager notebookManager;

    private ClueItem currentClue;
    private bool isInspecting;

    void Start()
    {
        inspectionUI.SetActive(false);
    }

    void Update()
    {
        if (!isInspecting || currentClue == null || SettingsManager.Instance == null) return;

        HandleRotation();
        HandleInput();
    }

    private void HandleRotation()
    {
        float rotX = Input.GetAxis("Mouse X") * 5f;
        float rotY = Input.GetAxis("Mouse Y") * 5f;
        currentClue.transform.Rotate(player.playerCamera.transform.up, -rotX, Space.World);
        currentClue.transform.Rotate(player.playerCamera.transform.right, rotY, Space.World);
    }

    private void HandleInput()
    {
        if (SettingsManager.Instance.GetKeyDown(SettingsManager.InspectPut))
        {
            PutBack();
        }
        else if (SettingsManager.Instance.GetKeyDown(SettingsManager.InspectTake) && currentClue.canBePickedUp)
        {
            TakeItem();
        }
    }

    public void StartInspection(ClueItem clue)
    {
        currentClue = clue;
        isInspecting = true;
        player.canMove = false;

        inspectionUI.SetActive(true);
        inspectionText.text = clue.clueText;
        currentClue.transform.position = inspectPoint.position;
    }

    private void PutBack()
    {
        currentClue.transform.position = currentClue.originalPosition;
        currentClue.transform.rotation = currentClue.originalRotation;
        EndInspection();
    }

    private void TakeItem()
    {
        notebookManager.AddRecord(currentClue);
        Destroy(currentClue.gameObject);
        EndInspection();
    }

    private void EndInspection()
    {
        isInspecting = false;
        currentClue = null;
        inspectionUI.SetActive(false);
        Invoke(nameof(EnablePlayerMovement), 0.1f);
    }

    private void EnablePlayerMovement() => player.canMove = true;
}
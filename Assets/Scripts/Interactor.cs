using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Настройки")]
    public float interactRange = 3.0f;

    [Header("Менеджеры")]
    public NotebookManager notebookManager;
    public InspectionManager inspectionManager;
    public PlayerController player;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Time.timeScale == 0 || !player.canMove || SettingsManager.Instance == null) return;

        if (SettingsManager.Instance.GetKeyDown(SettingsManager.Interact))
        {
            PerformRaycast();
        }
    }

    private void PerformRaycast()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.TryGetComponent(out ClueItem clue))
            {
                HandleClueInteraction(clue);
            }
        }
    }

    private void HandleClueInteraction(ClueItem clue)
    {
        switch (clue.type)
        {
            case ClueItem.ClueType.Inspectable3D:
                inspectionManager.StartInspection(clue);
                break;
            case ClueItem.ClueType.StaticText:
                notebookManager.AddRecord(clue);
                if (clue.destroyAfterInteraction) Destroy(clue.gameObject);
                break;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text;

public class NotebookManager : MonoBehaviour
{
    [Header("UI ›ÎÂÏÂÌÚ˚")]
    public GameObject notebookPanel;
    public TextMeshProUGUI notesText;

    private bool isOpen = false;
    private List<NoteRecord> recordsList = new List<NoteRecord>();

    private class NoteRecord
    {
        public string currentText;
        public string corruptedText;
        public bool canCorrupt;
        public bool isAlreadyCorrupted;
        public string triggerID;
    }

    void Start()
    {
        notebookPanel.SetActive(false);
        UpdateNotebookUI();
    }

    void Update()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.GetKeyDown(SettingsManager.Notebook))
        {
            ToggleNotebook();
        }
    }

    private void ToggleNotebook()
    {
        isOpen = !isOpen;
        notebookPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen) UpdateNotebookUI();
    }

    public void AddRecord(ClueItem clue)
    {
        recordsList.Add(new NoteRecord
        {
            currentText = clue.clueText,
            corruptedText = clue.corruptedText,
            canCorrupt = clue.isCorruptible,
            isAlreadyCorrupted = false,
            triggerID = clue.corruptionTriggerID
        });
        UpdateNotebookUI();
    }

    public void ActivateStoryTrigger(string triggeredID)
    {
        bool textChanged = false;

        foreach (var record in recordsList)
        {
            if (record.canCorrupt && !record.isAlreadyCorrupted && record.triggerID == triggeredID)
            {
                record.currentText = record.corruptedText;
                record.isAlreadyCorrupted = true;
                textChanged = true;
            }
        }

        if (textChanged && isOpen) UpdateNotebookUI();
    }

    private void UpdateNotebookUI()
    {
        StringBuilder sb = new StringBuilder("Ã¿“≈–»¿À€ ƒ≈À¿:\n\n");
        foreach (var record in recordsList)
        {
            sb.AppendLine($"- {record.currentText}\n");
        }
        notesText.text = sb.ToString();
    }
}
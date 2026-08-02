using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text;
using NodeZero.Core;
using NodeZero.Interaction;

namespace NodeZero.UI
{
    public class NotebookManager : MonoBehaviour
    {
        [Header("UI Элементы")]
        [SerializeField] private GameObject _notebookPanel;
        [SerializeField] private TextMeshProUGUI _notesText;

        private bool _isOpen = false;

        private List<NoteRecord> _recordsList = new List<NoteRecord>(16);
        private StringBuilder _sb = new StringBuilder(512);

        private struct NoteRecord
        {
            public string currentText;
            public string corruptedText;
            public bool canCorrupt;
            public bool isAlreadyCorrupted;
            public string triggerID;
        }

        private void OnEnable()
        {
            EventBus.OnStoryTriggered += ActivateStoryTrigger;
            EventBus.OnClueCollected += AddRecord; // Подписка на сбор улик
        }

        private void OnDisable()
        {
            EventBus.OnStoryTriggered -= ActivateStoryTrigger;
            EventBus.OnClueCollected -= AddRecord;
        }

        private void Start()
        {
            _notebookPanel.SetActive(false);
            UpdateNotebookUI();
        }

        private void Update()
        {
            if (SettingsManager.Instance != null && SettingsManager.Instance.IsNotebookPressed())
            {
                ToggleNotebook();
            }
        }

        private void ToggleNotebook()
        {
            _isOpen = !_isOpen;
            _notebookPanel.SetActive(_isOpen);

            if (_isOpen) UpdateNotebookUI();
        }

        public void AddRecord(ClueData data)
        {
            // Проверка на дубликаты, чтобы не добавлять одну и ту же улику дважды
            for (int i = 0; i < _recordsList.Count; i++)
            {
                if (_recordsList[i].currentText == data.clueText || _recordsList[i].corruptedText == data.corruptedText)
                {
                    return;
                }
            }

            _recordsList.Add(new NoteRecord
            {
                currentText = data.clueText,
                corruptedText = data.corruptedText,
                canCorrupt = data.isCorruptible,
                isAlreadyCorrupted = false,
                triggerID = data.corruptionTriggerID
            });

            if (_isOpen) UpdateNotebookUI();
        }

        private void ActivateStoryTrigger(string triggeredID)
        {
            bool textChanged = false;

            for (int i = 0; i < _recordsList.Count; i++)
            {
                NoteRecord record = _recordsList[i];
                if (record.canCorrupt && !record.isAlreadyCorrupted && record.triggerID == triggeredID)
                {
                    record.currentText = record.corruptedText;
                    record.isAlreadyCorrupted = true;
                    _recordsList[i] = record;
                    textChanged = true;
                }
            }

            if (textChanged && _isOpen) UpdateNotebookUI();
        }

        private void UpdateNotebookUI()
        {
            _sb.Clear();
            _sb.Append("МАТЕРИАЛЫ ДЕЛА:\n\n");

            for (int i = 0; i < _recordsList.Count; i++)
            {
                _sb.Append("- ").Append(_recordsList[i].currentText).Append("\n\n");
            }

            _notesText.text = _sb.ToString();
        }
    }
}
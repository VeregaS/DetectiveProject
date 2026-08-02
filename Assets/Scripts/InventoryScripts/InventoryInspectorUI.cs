using TMPro;
using UnityEngine;

namespace NodeZero.Inventory
{
    public sealed class InventoryInspectorUI : MonoBehaviour
    {
        private static InventoryInspectorUI _instance;

        [SerializeField] private GameObject _inspectorPanel;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private void Awake()
        {
            _instance = this;
            Hide();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public static void ShowSelected(InventoryItemData data)
        {
            if (_instance == null || data == null) return;
            _instance.Show(data);
        }

        public void Hide()
        {
            if (_inspectorPanel != null) _inspectorPanel.SetActive(false);
        }

        private void Show(InventoryItemData data)
        {
            if (_titleText != null) _titleText.text = data.DisplayName;
            if (_descriptionText != null) _descriptionText.text = data.Description;
            if (_inspectorPanel != null) _inspectorPanel.SetActive(true);
        }
    }
}

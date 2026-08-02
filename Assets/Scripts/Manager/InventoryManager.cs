using System;
using UnityEngine;
using NodeZero.Core;
using NodeZero.Interaction;

namespace NodeZero.Inventory
{
    public sealed class InventoryManager : MonoBehaviour
    {
        [Header("Размер чемодана")]
        [Min(1)]
        [SerializeField] private int _width = 10;
        [Min(1)]
        [SerializeField] private int _height = 8;

        [Header("Ссылки")]
        [SerializeField] private GameObject _inventoryRoot;
        [SerializeField] private InventoryGridView _gridView;
        [SerializeField] private PlayerController _player;

        private CursorLockMode _previousCursorLock;
        private bool _previousCursorVisible;
        private bool _movementWasEnabled;

        public InventoryGrid Grid { get; private set; }
        public bool IsOpen { get; private set; }

        public event Action InventoryChanged;

        private void Awake()
        {
            if (_inventoryRoot == null || _gridView == null)
            {
                Debug.LogError("InventoryManager: назначьте Inventory Root и Grid View в Inspector.", this);
                enabled = false;
                return;
            }

            Grid = new InventoryGrid(Mathf.Max(1, _width), Mathf.Max(1, _height));
            _gridView.Bind(this);
            _inventoryRoot.SetActive(false);
        }

        private void Update()
        {
            if (SettingsManager.Instance == null || !SettingsManager.Instance.IsInventoryPressed()) return;
            if (!IsOpen && InspectionManager.IsInspecting) return;

            SetOpen(!IsOpen);
        }

        private void OnDisable()
        {
            if (IsOpen)
            {
                SetOpen(false);
            }
        }

        public bool TryAdd(InventoryItemData data)
        {
            if (!Grid.TryAdd(data, out _)) return false;

            InventoryChanged?.Invoke();
            return true;
        }

        public bool TryMove(InventoryItem item, Vector2Int position)
        {
            if (!Grid.TryMove(item, position, item.IsRotated)) return false;

            InventoryChanged?.Invoke();
            return true;
        }

        public bool TryRotate(InventoryItem item)
        {
            if (!Grid.TryRotate(item)) return false;

            InventoryChanged?.Invoke();
            return true;
        }

        public void SetOpen(bool isOpen)
        {
            if (IsOpen == isOpen) return;

            IsOpen = isOpen;
            _inventoryRoot.SetActive(isOpen);

            if (isOpen)
            {
                _movementWasEnabled = _player == null || _player.canMove;
                _previousCursorLock = Cursor.lockState;
                _previousCursorVisible = Cursor.visible;
                EventBus.RaisePlayerStateChanged(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _gridView.Refresh();
            }
            else
            {
                EventBus.RaisePlayerStateChanged(_movementWasEnabled);
                Cursor.lockState = _previousCursorLock;
                Cursor.visible = _previousCursorVisible;
            }
        }
    }
}
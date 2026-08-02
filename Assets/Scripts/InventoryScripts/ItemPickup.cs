using UnityEngine;
using NodeZero.Core;

namespace NodeZero.Inventory
{
    public sealed class ItemPickup : MonoBehaviour
    {
        [SerializeField] private InventoryItemData _data;
        [SerializeField] private InventoryManager _inventory;
        [SerializeField] private bool _inspectBeforePickup = true;

        public InventoryItemData Data => _data;
        public Vector3 OriginalPosition { get; private set; }
        public Quaternion OriginalRotation { get; private set; }

        private void Awake()
        {
            OriginalPosition = transform.position;
            OriginalRotation = transform.rotation;
        }

        public void Interact()
        {
            if (_data == null)
            {
                Debug.LogError($"У объекта '{name}' не назначен InventoryItemData.", this);
                return;
            }

            if (_inspectBeforePickup)
            {
                EventBus.RaiseItemInspectionStarted(this);
            }
            else
            {
                TryCollect();
            }
        }

        public bool TryCollect()
        {
            if (_inventory == null)
            {
                Debug.LogError($"У объекта '{name}' не назначен InventoryManager.", this);
                return false;
            }

            if (!_inventory.TryAdd(_data)) return false;

            gameObject.SetActive(false);
            return true;
        }
    }
}
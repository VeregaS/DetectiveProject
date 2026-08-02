using UnityEngine;

namespace NodeZero.Inventory
{
    public abstract class InventoryItemData : ScriptableObject
    {
        [SerializeField] private string itemName = "Item";
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [Min(1)]
        [SerializeField] private int width = 1;
        [Min(1)]
        [SerializeField] private int height = 1;
        [SerializeField] private bool canRotate = true;

        public string DisplayName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public int Width => Mathf.Max(1, width);
        public int Height => Mathf.Max(1, height);
        public bool CanRotate => canRotate;
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "Node Zero/Inventory Item")]
    public sealed class ItemData : InventoryItemData
    {
    }
}

using UnityEngine;

namespace NodeZero.Inventory
{
    public sealed class InventoryItem
    {
        public InventoryItem(InventoryItemData data)
        {
            Data = data;
            Position = new Vector2Int(-1, -1);
        }

        public InventoryItemData Data { get; }
        public Vector2Int Position { get; internal set; }
        public bool IsRotated { get; internal set; }
        public int Width => IsRotated ? Data.Height : Data.Width;
        public int Height => IsRotated ? Data.Width : Data.Height;
    }
}

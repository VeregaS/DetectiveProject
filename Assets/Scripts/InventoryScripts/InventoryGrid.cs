using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeZero.Inventory
{
    public sealed class InventoryGrid
    {
        private readonly InventoryItem[,] _cells;
        private readonly List<InventoryItem> _items = new List<InventoryItem>();

        public InventoryGrid(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            _cells = new InventoryItem[width, height];
        }

        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<InventoryItem> Items => _items;

        public InventoryItem GetItemAt(int x, int y)
        {
            return IsInside(x, y) ? _cells[x, y] : null;
        }

        public bool TryAdd(InventoryItemData data, out InventoryItem item)
        {
            item = null;
            if (data == null) return false;

            InventoryItem candidate = new InventoryItem(data);
            if (!TryFindSpace(candidate, out Vector2Int position, out bool rotated)) return false;

            Place(candidate, position, rotated);
            item = candidate;
            return true;
        }

        public bool TryMove(InventoryItem item, Vector2Int position, bool rotated)
        {
            if (item == null || !_items.Contains(item)) return false;
            if (!CanPlace(item, position, rotated)) return false;

            Place(item, position, rotated);
            return true;
        }

        public bool TryRotate(InventoryItem item)
        {
            if (item == null || !item.Data.CanRotate || !_items.Contains(item)) return false;
            return TryMove(item, item.Position, !item.IsRotated);
        }

        public bool Remove(InventoryItem item)
        {
            if (item == null || !_items.Remove(item)) return false;

            ClearCells(item);
            item.Position = new Vector2Int(-1, -1);
            item.IsRotated = false;
            return true;
        }

        public bool CanPlace(InventoryItem item, Vector2Int position, bool rotated)
        {
            if (item == null || item.Data == null) return false;

            int itemWidth = rotated ? item.Data.Height : item.Data.Width;
            int itemHeight = rotated ? item.Data.Width : item.Data.Height;

            if (position.x < 0 || position.y < 0 ||
                position.x + itemWidth > Width || position.y + itemHeight > Height)
            {
                return false;
            }

            for (int y = position.y; y < position.y + itemHeight; y++)
            {
                for (int x = position.x; x < position.x + itemWidth; x++)
                {
                    InventoryItem occupant = _cells[x, y];
                    if (occupant != null && occupant != item) return false;
                }
            }

            return true;
        }

        private bool TryFindSpace(InventoryItem item, out Vector2Int position, out bool rotated)
        {
            if (TryFindSpace(item, false, out position))
            {
                rotated = false;
                return true;
            }

            if (item.Data.CanRotate && item.Data.Width != item.Data.Height &&
                TryFindSpace(item, true, out position))
            {
                rotated = true;
                return true;
            }

            position = default;
            rotated = false;
            return false;
        }

        private bool TryFindSpace(InventoryItem item, bool rotated, out Vector2Int position)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector2Int candidate = new Vector2Int(x, y);
                    if (!CanPlace(item, candidate, rotated)) continue;

                    position = candidate;
                    return true;
                }
            }

            position = default;
            return false;
        }

        private void Place(InventoryItem item, Vector2Int position, bool rotated)
        {
            ClearCells(item);

            if (!_items.Contains(item))
            {
                _items.Add(item);
            }

            item.Position = position;
            item.IsRotated = rotated;

            for (int y = position.y; y < position.y + item.Height; y++)
            {
                for (int x = position.x; x < position.x + item.Width; x++)
                {
                    _cells[x, y] = item;
                }
            }
        }

        private void ClearCells(InventoryItem item)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_cells[x, y] == item)
                    {
                        _cells[x, y] = null;
                    }
                }
            }
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
}
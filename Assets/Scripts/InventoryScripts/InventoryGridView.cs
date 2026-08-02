using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeZero.Inventory
{
    public sealed class InventoryGridView : MonoBehaviour
    {
        [SerializeField] private RectTransform _gridRoot;
        [SerializeField] private InventoryItemView _itemViewPrefab;
        [SerializeField] private Vector2 _cellSize = new Vector2(64f, 64f);
        [SerializeField] private Vector2 _spacing = new Vector2(4f, 4f);

        private readonly List<InventoryItemView> _spawnedViews = new List<InventoryItemView>();
        private InventoryManager _manager;

        public void Bind(InventoryManager manager)
        {
            if (_manager != null)
            {
                _manager.InventoryChanged -= Refresh;
            }

            _manager = manager;

            if (_manager != null)
            {
                _manager.InventoryChanged += Refresh;
                ResizeGrid();
            }
        }

        private void OnDestroy()
        {
            if (_manager != null)
            {
                _manager.InventoryChanged -= Refresh;
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < _spawnedViews.Count; i++)
            {
                if (_spawnedViews[i] != null)
                {
                    Destroy(_spawnedViews[i].gameObject);
                }
            }

            _spawnedViews.Clear();
            if (_manager == null || _itemViewPrefab == null) return;

            IReadOnlyList<InventoryItem> items = _manager.Grid.Items;
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItemView view = Instantiate(_itemViewPrefab, _gridRoot);
                view.Bind(this, items[i]);
                PositionView(view.RectTransform, items[i]);
                _spawnedViews.Add(view);
            }
        }

        public bool TryMove(InventoryItem item, PointerEventData eventData, Vector2Int grabOffset)
        {
            if (_manager == null || !TryGetCell(eventData, out Vector2Int cell)) return false;
            return _manager.TryMove(item, cell - grabOffset);
        }

        public bool TryRotate(InventoryItem item)
        {
            return _manager != null && _manager.TryRotate(item);
        }

        private void ResizeGrid()
        {
            if (_manager == null || _gridRoot == null) return;

            _gridRoot.sizeDelta = new Vector2(
                _manager.Grid.Width * _cellSize.x + (_manager.Grid.Width - 1) * _spacing.x,
                _manager.Grid.Height * _cellSize.y + (_manager.Grid.Height - 1) * _spacing.y);
        }

        private void PositionView(RectTransform rectTransform, InventoryItem item)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(
                item.Position.x * (_cellSize.x + _spacing.x),
                -item.Position.y * (_cellSize.y + _spacing.y));
            rectTransform.sizeDelta = new Vector2(
                item.Width * _cellSize.x + (item.Width - 1) * _spacing.x,
                item.Height * _cellSize.y + (item.Height - 1) * _spacing.y);
        }

        private bool TryGetCell(PointerEventData eventData, out Vector2Int cell)
        {
            cell = default;
            if (_gridRoot == null || _manager == null) return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _gridRoot,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = _gridRoot.rect;
            float fromLeft = localPoint.x - rect.xMin;
            float fromTop = rect.yMax - localPoint.y;
            if (fromLeft < 0f || fromTop < 0f || fromLeft >= rect.width || fromTop >= rect.height)
            {
                return false;
            }

            int x = Mathf.FloorToInt(fromLeft / (_cellSize.x + _spacing.x));
            int y = Mathf.FloorToInt(fromTop / (_cellSize.y + _spacing.y));
            if (x < 0 || y < 0 || x >= _manager.Grid.Width || y >= _manager.Grid.Height)
            {
                return false;
            }

            cell = new Vector2Int(x, y);
            return true;
        }
    }
}
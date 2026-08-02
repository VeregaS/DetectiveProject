using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NodeZero.Inventory
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform _rootRect;
        [SerializeField] private RectTransform _graphicRect;
        [SerializeField] private Image _icon;

        private InventoryGridView _gridView;
        private InventoryItem _item;
        private Transform _originalParent;
        private Vector2 _originalPosition;
        private Vector2Int _grabOffset;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public RectTransform RectTransform => _rootRect != null ? _rootRect : (RectTransform)transform;

        public void Bind(InventoryGridView gridView, InventoryItem item)
        {
            _gridView = gridView;
            _item = item;
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (_icon != null)
            {
                _icon.sprite = item.Data.Icon;
                _icon.preserveAspect = true;
            }

            if (_graphicRect != null)
            {
                _graphicRect.localRotation = Quaternion.Euler(0f, 0f, item.IsRotated ? -90f : 0f);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_item == null) return;

            _originalParent = RectTransform.parent;
            _originalPosition = RectTransform.anchoredPosition;
            _grabOffset = GetGrabOffset(eventData);
            RectTransform.SetParent(_canvas != null ? _canvas.transform : _originalParent, true);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_item == null) return;
            RectTransform.position += (Vector3)eventData.delta / (_canvas != null ? _canvas.scaleFactor : 1f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_item == null) return;

            RectTransform.SetParent(_originalParent, false);
            _canvasGroup.blocksRaycasts = true;
            if (!_gridView.TryMove(_item, eventData, _grabOffset))
            {
                RectTransform.anchoredPosition = _originalPosition;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_item == null) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _gridView.TryRotate(_item);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                InventoryInspectorUI.ShowSelected(_item.Data);
            }
        }

        private Vector2Int GetGrabOffset(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 point))
            {
                return Vector2Int.zero;
            }

            Rect rect = RectTransform.rect;
            float normalizedX = Mathf.Clamp01((point.x - rect.xMin) / rect.width);
            float normalizedY = Mathf.Clamp01((rect.yMax - point.y) / rect.height);
            return new Vector2Int(
                Mathf.Min(_item.Width - 1, Mathf.FloorToInt(normalizedX * _item.Width)),
                Mathf.Min(_item.Height - 1, Mathf.FloorToInt(normalizedY * _item.Height)));
        }
    }

    public sealed class InventoryItemUI : InventoryItemView
    {
    }
}

using UnityEngine;
using TMPro;
using NodeZero.Core;
using NodeZero.Inventory;

namespace NodeZero.Interaction
{
    public class InspectionManager : MonoBehaviour
    {
        [Header("UI Осмотра")]
        [SerializeField] private GameObject _inspectionUI;
        [SerializeField] private TextMeshProUGUI _inspectionText;

        [Header("Настройки камеры")]
        [SerializeField] private Transform _inspectPoint;
        [SerializeField] private PlayerController _player;

        [Header("Подсказки UI")]
        [SerializeField] private UI.DynamicKeyText _inspectionKeysUI;
        [SerializeField] private string _inventoryFullText = "В инвентаре недостаточно места.";

        private ClueItem _currentClue;
        private ItemPickup _currentItem;

        public static bool IsInspecting { get; private set; }

        private void OnEnable()
        {
            EventBus.OnInspectionStarted += StartClueInspection;
            EventBus.OnItemInspectionStarted += StartItemInspection;
        }

        private void OnDisable()
        {
            EventBus.OnInspectionStarted -= StartClueInspection;
            EventBus.OnItemInspectionStarted -= StartItemInspection;
            CancelInvoke(nameof(EnablePlayerMovement));

            if (IsInspecting)
            {
                IsInspecting = false;
                _currentClue = null;
                _currentItem = null;

                if (_player != null)
                {
                    _player.canMove = true;
                }
            }
        }

        private void Start()
        {
            _inspectionUI.SetActive(false);
        }

        private void Update()
        {
            if (!IsInspecting || (_currentClue == null && _currentItem == null) || SettingsManager.Instance == null) return;

            HandleRotation();
            HandleInput();
        }

        private void HandleRotation()
        {
            Vector2 lookDelta = SettingsManager.Instance.GetLookDelta();
            float rotX = lookDelta.x * 0.2f;
            float rotY = lookDelta.y * 0.2f;

            Transform target = _currentClue != null
                ? _currentClue.transform
                : _currentItem.transform;

            target.Rotate(_player.playerCamera.transform.up, -rotX, Space.World);
            target.Rotate(_player.playerCamera.transform.right, rotY, Space.World);
        }

        private void HandleInput()
        {
            if (SettingsManager.Instance.IsInspectPutBackPressed())
            {
                PutBack();
            }
            else if (SettingsManager.Instance.IsInspectTakePressed())
            {
                TakeItem();
            }
        }

        // Логика начала осмотра для улик
        private void StartClueInspection(ClueItem clue)
        {
            if (clue == null || clue.Data == null) return;

            _currentClue = clue;
            _currentItem = null;
            SetupInspectionUI(clue.transform, clue.Data.clueText);
        }

        private void StartItemInspection(ItemPickup item)
        {
            if (item == null || item.Data == null) return;

            _currentClue = null;
            _currentItem = item;
            SetupInspectionUI(item.transform, item.Data.Description);
        }

        // Общий метод для позиционирования и включения интерфейса
        private void SetupInspectionUI(Transform objTransform, string text)
        {
            IsInspecting = true;
            CancelInvoke(nameof(EnablePlayerMovement));
            _player.canMove = false;

            _inspectionUI.SetActive(true);
            _inspectionText.text = text;
            objTransform.position = _inspectPoint.position;

            if (_inspectionKeysUI != null) _inspectionKeysUI.SetVisibility(true);
        }

        private void PutBack()
        {
            if (_currentClue != null)
            {
                _currentClue.transform.position = _currentClue.OriginalPosition;
                _currentClue.transform.rotation = _currentClue.OriginalRotation;
            }
            else if (_currentItem != null)
            {
                _currentItem.transform.position = _currentItem.OriginalPosition;
                _currentItem.transform.rotation = _currentItem.OriginalRotation;
            }

            EndInspection();
        }

        private void TakeItem()
        {
            if (_currentClue != null && _currentClue.Data.canBePickedUp)
            {
                EventBus.RaiseClueCollected(_currentClue.Data);
                _currentClue.gameObject.SetActive(false);
                EndInspection();
            }
            else if (_currentItem != null)
            {
                if (_currentItem.TryCollect())
                {
                    EndInspection();
                }
                else
                {
                    _inspectionText.text = _inventoryFullText;
                }
            }
        }

        private void EndInspection()
        {
            IsInspecting = false;
            _currentClue = null;
            _currentItem = null;
            _inspectionUI.SetActive(false);

            if (_inspectionKeysUI != null) _inspectionKeysUI.SetVisibility(false);

            Invoke(nameof(EnablePlayerMovement), 0.1f);
        }

        private void EnablePlayerMovement() => _player.canMove = true;
    }
}
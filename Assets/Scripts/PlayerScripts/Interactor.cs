using UnityEngine;
using NodeZero.Core;
using NodeZero.Inventory;

namespace NodeZero.Interaction
{
    [RequireComponent(typeof(Camera))]
    public class Interactor : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float _interactRange = 3.0f;
        [SerializeField] private LayerMask _interactableLayer;

        [Header("Ссылки")]
        [SerializeField] private PlayerController _player;

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Time.timeScale == 0 || !_player.canMove || SettingsManager.Instance == null) return;

            if (SettingsManager.Instance.IsInteractPressed())
            {
                PerformRaycast();
            }
        }

        private void PerformRaycast()
        {
            Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactableLayer))
            {
                ClueItem clue = hit.collider.GetComponentInParent<ClueItem>();
                if (clue != null)
                {
                    HandleClueInteraction(clue);
                }
                else
                {
                    ItemPickup itemPickup = hit.collider.GetComponentInParent<ItemPickup>();
                    if (itemPickup != null)
                    {
                        itemPickup.Interact();
                    }
                }
            }
        }

        private void HandleClueInteraction(ClueItem clue)
        {
            if (clue.Data == null)
            {
                Debug.LogError($"У объекта '{clue.name}' не назначен ClueData.", clue);
                return;
            }

            switch (clue.Data.type)
            {
                case ClueType.Inspectable3D:
                    EventBus.RaiseInspectionStarted(clue);
                    break;
                case ClueType.StaticText:
                    EventBus.RaiseClueCollected(clue.Data);
                    if (clue.Data.destroyAfterInteraction)
                    {
                        clue.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}
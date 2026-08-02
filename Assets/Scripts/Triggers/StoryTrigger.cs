using UnityEngine;

namespace NodeZero.Core
{
    public class StoryTrigger : MonoBehaviour
    {
        [Header("Настройки Триггера")]
        [SerializeField] private string _triggerID = "event_01";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.RaiseStoryTriggered(_triggerID);

                // Деактивация вместо Destroy для предотвращения аллокаций
                gameObject.SetActive(false);
            }
        }
    }
}
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [Header("Настройки Триггера")]
    public string triggerID = "event_01"; // Должно совпадать с ID в улике
    public NotebookManager notebookManager;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в зону вошел именно игрок
        if (other.CompareTag("Player"))
        {
            if (notebookManager != null)
            {
                // Отправляем сигнал о событии в блокнот
                notebookManager.ActivateStoryTrigger(triggerID);
            }

            // Удаляем этот триггер, чтобы он сработал только один раз
            Destroy(gameObject);
        }
    }
}
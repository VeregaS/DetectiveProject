using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DynamicKeyText : MonoBehaviour
{
    [TextArea(2, 5)]
    [Tooltip("Используйте фигурные скобки для подстановки. Пример: '{InspectPutBack} - Вернуть | {InspectTake} - Забрать'")]
    public string templateText = "{InspectPutBack} - Вернуть | {InspectTake} - Забрать";

    private TextMeshProUGUI textComponent;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Автоматически ищем или добавляем CanvasGroup для быстрой и плавной видимости
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        UpdateText();
        SetVisibility(false); // Скрываем при старте
    }

    public void UpdateText()
    {
        if (SettingsManager.Instance == null) return;

        string finalText = templateText;

        foreach (var kvp in SettingsManager.Instance.keys)
        {
            string tag = "{" + kvp.Key + "}";
            if (finalText.Contains(tag))
            {
                finalText = finalText.Replace(tag, SettingsManager.Instance.GetKeyName(kvp.Key));
            }
        }

        textComponent.text = finalText;
    }

    // Управляем видимостью через пропуск света/видимость группы (нулевой фриз)
    public void SetVisibility(bool isVisible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
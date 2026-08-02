using UnityEngine;

public class ClueItem : MonoBehaviour
{
    public enum ClueType { Inspectable3D, StaticText }

    [Header("Настройки улики")]
    public ClueType type = ClueType.Inspectable3D;
    
    [TextArea(3, 5)]
    public string clueText = "Оригинальный текст...";

    [Header("Сюжетный Газлайтинг")]
    public bool isCorruptible = false; // Может ли сломаться?
    public string corruptionTriggerID = "event_01"; // Кодовое слово триггера
    [TextArea(3, 5)]
    public string corruptedText = "Искаженный текст...";

    [Header("Флаги")]
    public bool canBePickedUp = true; 
    public bool destroyAfterInteraction = false; 

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
}
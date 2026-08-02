using UnityEngine;

namespace NodeZero.Interaction
{
    public enum ClueType { Inspectable3D, StaticText }

    [CreateAssetMenu(fileName = "New Clue Data", menuName = "NodeZero/Clue Data")]
    public class ClueData : ScriptableObject
    {
        [Header("Настройки улики")]
        public ClueType type = ClueType.Inspectable3D;

        [TextArea(3, 5)]
        public string clueText = "Оригинальный текст...";

        [Header("Сюжетный Газлайтинг")]
        public bool isCorruptible = false;
        public string corruptionTriggerID = "event_01";

        [TextArea(3, 5)]
        public string corruptedText = "Искаженный текст...";

        [Header("Флаги")]
        public bool canBePickedUp = true;
        public bool destroyAfterInteraction = false;
    }
}
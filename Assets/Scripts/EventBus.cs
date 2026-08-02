using System;
using NodeZero.Interaction;
using NodeZero.Inventory;

namespace NodeZero.Core
{
    public static class EventBus
    {
        public static event Action<string> OnStoryTriggered;
        public static event Action<bool> OnPlayerStateChanged;

        //  
        public static event Action<ClueItem> OnInspectionStarted;
        public static event Action<ItemPickup> OnItemInspectionStarted;
        public static event Action<ClueData> OnClueCollected;

        //     

        public static void RaiseStoryTriggered(string triggerID) => OnStoryTriggered?.Invoke(triggerID);
        public static void RaisePlayerStateChanged(bool canMove) => OnPlayerStateChanged?.Invoke(canMove);
        public static void RaiseInspectionStarted(ClueItem clue) => OnInspectionStarted?.Invoke(clue);
        public static void RaiseItemInspectionStarted(ItemPickup item) => OnItemInspectionStarted?.Invoke(item);
        public static void RaiseClueCollected(ClueData clueData) => OnClueCollected?.Invoke(clueData);

    }
}
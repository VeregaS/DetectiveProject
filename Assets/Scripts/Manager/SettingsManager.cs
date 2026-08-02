using UnityEngine;
using UnityEngine.InputSystem;

namespace NodeZero.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        public InputSystem_Actions Input { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Input = new InputSystem_Actions();
                Input.Enable();
                Input.UI.Enable();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Input?.Disable();
            }
        }

        // Обертки для безопасного доступа к текущим действиям
        public Vector2 GetMoveDelta() => Input.Player.Move.ReadValue<Vector2>();
        public Vector2 GetLookDelta() => Input.Player.Look.ReadValue<Vector2>();
        public float GetLeanDelta() => Input.Player.Lean.ReadValue<float>();

        public bool IsCrouchPressed() => Input.Player.Crouch.WasPressedThisFrame();
        public bool IsInteractPressed() => Input.Player.Interact.WasPressedThisFrame();
        public bool IsFlashlightPressed() => Input.Player.Flashlight.WasPressedThisFrame();
        public bool IsNotebookPressed() => Input.Player.Notebook.WasPressedThisFrame();
        public bool IsInspectTakePressed() => Input.Player.InspectTake.WasPressedThisFrame();
        public bool IsInspectPutBackPressed() => Input.Player.InspectPutBack.WasPressedThisFrame();
        public bool IsInventoryPressed() => Input.Player.Inventory.WasPressedThisFrame();

        // Универсальный метод получения названия клавиши для UI
        public string GetKeyName(string actionName)
        {
            InputAction action = Input.asset.FindAction(actionName);
            if (action == null || action.bindings.Count == 0) return "?";

            string displayString = action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);

            // Локализация кнопок мыши для совместимости с прежней логикой
            return displayString switch
            {
                "LMB" => "ЛКМ",
                "RMB" => "ПКМ",
                "MMB" => "СКМ",
                _ => displayString
            };
        }
    }
}
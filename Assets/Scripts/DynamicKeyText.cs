using UnityEngine;
using TMPro;
using System.Text;
using NodeZero.Core;

namespace NodeZero.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DynamicKeyText : MonoBehaviour
    {
        [TextArea(2, 5)]
        [Tooltip("Используйте фигурные скобки для подстановки. Пример: '{InspectPutBack} - Вернуть'")]
        [SerializeField] private string _templateText = "{InspectPutBack} - Вернуть | {InspectTake} - Забрать";

        private TextMeshProUGUI _textComponent;
        private CanvasGroup _canvasGroup;
        private StringBuilder _sb = new StringBuilder(128);

        // Список всех потенциальных действий для кэширования тегов
        private static readonly string[] _actionNames = {
            "Interact", "Flashlight", "Notebook", "InspectTake", "InspectPutBack", "Crouch", "Jump", "Sprint"
        };
        private static string[] _cachedTags;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            InitializeTags();
            SetVisibility(false);
        }

        private void Start()
        {
            UpdateText();
        }

        private void InitializeTags()
        {
            if (_cachedTags == null)
            {
                _cachedTags = new string[_actionNames.Length];
                for (int i = 0; i < _actionNames.Length; i++)
                {
                    _cachedTags[i] = "{" + _actionNames[i] + "}";
                }
            }
        }

        public void UpdateText()
        {
            if (SettingsManager.Instance == null) return;

            _sb.Clear();
            _sb.Append(_templateText);

            for (int i = 0; i < _actionNames.Length; i++)
            {
                _sb.Replace(_cachedTags[i], SettingsManager.Instance.GetKeyName(_actionNames[i]));
            }

            _textComponent.text = _sb.ToString();
        }

        public void SetVisibility(bool isVisible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isVisible ? 1f : 0f;
                _canvasGroup.blocksRaycasts = isVisible;
            }
        }
    }
}
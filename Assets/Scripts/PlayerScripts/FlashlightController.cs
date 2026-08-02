using UnityEngine;
using NodeZero.Core;

namespace NodeZero.Interaction
{
    [RequireComponent(typeof(Light))]
    public class FlashlightController : MonoBehaviour
    {
        private Light _flashlight;

        private void Start()
        {
            _flashlight = GetComponent<Light>();
        }

        private void Update()
        {
            if (SettingsManager.Instance != null && SettingsManager.Instance.IsFlashlightPressed())
            {
                _flashlight.enabled = !_flashlight.enabled;
            }
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    private Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
    }

    void Update()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.GetKeyDown(SettingsManager.Flashlight))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
}
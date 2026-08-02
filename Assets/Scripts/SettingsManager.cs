using UnityEngine;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Константы действий для защиты от опечаток
    public const string MoveFwd = "MoveForward";
    public const string MoveBck = "MoveBackward";
    public const string MoveLft = "MoveLeft";
    public const string MoveRgt = "MoveRight";
    public const string Crouch = "Crouch";
    public const string LeanLft = "LeanLeft";
    public const string LeanRgt = "LeanRight";
    public const string Interact = "Interact";
    public const string Flashlight = "Flashlight";
    public const string Notebook = "Notebook";
    public const string InspectTake = "InspectTake";
    public const string InspectPut = "InspectPutBack";

    public Dictionary<string, KeyCode> keys { get; private set; } = new Dictionary<string, KeyCode>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDefaultKeys();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadDefaultKeys()
    {
        keys[MoveFwd] = KeyCode.W;
        keys[MoveBck] = KeyCode.S;
        keys[MoveLft] = KeyCode.A;
        keys[MoveRgt] = KeyCode.D;
        keys[Crouch] = KeyCode.LeftControl;
        keys[LeanLft] = KeyCode.Q;
        keys[LeanRgt] = KeyCode.E;
        keys[Interact] = KeyCode.F;
        keys[Flashlight] = KeyCode.R;
        keys[Notebook] = KeyCode.Tab;
        keys[InspectTake] = KeyCode.F;
        keys[InspectPut] = KeyCode.Mouse1;
    }

    public string GetKeyName(string actionName)
    {
        if (!keys.ContainsKey(actionName)) return "?";

        KeyCode code = keys[actionName];
        return code switch
        {
            KeyCode.Mouse0 => "ЛКМ",
            KeyCode.Mouse1 => "ПКМ",
            KeyCode.Mouse2 => "СКМ",
            _ => code.ToString()
        };
    }

    // Врапперы для безопасного запроса ввода
    public bool GetKeyDown(string actionName) => keys.ContainsKey(actionName) && Input.GetKeyDown(keys[actionName]);
    public bool GetKey(string actionName) => keys.ContainsKey(actionName) && Input.GetKey(keys[actionName]);
}
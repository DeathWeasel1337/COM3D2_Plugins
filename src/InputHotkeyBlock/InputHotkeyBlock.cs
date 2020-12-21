using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class InputHotkeyBlock : BaseUnityPlugin
{
    public const string PluginGUID = "deathweasel.com3d2.inputhotkeyblock";
    public const string PluginName = "InputHotkeyBlock";
    public const string PluginVersion = "1.2";
    private static bool UIInputSelected;

    private void Awake() => Harmony.CreateAndPatchAll(typeof(Hooks));

    private void Update()
    {
        if (UIInput.selection == null)
            UIInputSelected = false;
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //Force UIInput fields to lose focus when clicking outside of them
            if (UICamera.hoveredObject != UIInput.selection.gameObject)
                Traverse.Create(UIInput.selection).Method("OnSelect", false).GetValue();
        }
        else
            UIInputSelected = UIInput.selection.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Called by the hooks, returns false to prevent the original method from running
    /// </summary>
    private static bool HotkeyBlock()
    {
        //GUI elements from some mods
        if (GUIUtility.keyboardControl > 0)
            return false;

        //NGUI UIInput
        if (UIInputSelected)
            return false;

        //Unity InputFields
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                return false;

        return true;
    }

    private static class Hooks
    {
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKey), typeof(KeyCode))]
        private static bool GetKeyCode(KeyCode key) => key == KeyCode.Return || key == KeyCode.Backspace || key == KeyCode.KeypadEnter || HotkeyBlock();
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKey), typeof(string))]
        private static bool GetKeyString() => HotkeyBlock();
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(KeyCode))]
        private static bool GetKeyDownCode(KeyCode key) => key == KeyCode.Return || key == KeyCode.Backspace || key == KeyCode.KeypadEnter || HotkeyBlock();
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(string))]
        private static bool GetKeyDownString() => HotkeyBlock();
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(KeyCode))]
        private static bool GetKeyUpCode(KeyCode key) => key == KeyCode.Return || key == KeyCode.Backspace || key == KeyCode.KeypadEnter || HotkeyBlock();
        [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(string))]
        private static bool GetKeyUpString() => HotkeyBlock();
    }
}

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace COM3D2_Plugins
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class InputHotkeyBlock : BaseUnityPlugin
    {
        public const string PluginGUID = "deathweasel.com3d2.inputhotkeyblock";
        public const string PluginName = "InputHotkeyBlock";
        public const string PluginVersion = "1.0";
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
            //UI elements from some mods
            if (GUIUtility.keyboardControl > 0)
                return false;

            //NGUI UIInput
            if (UIInputSelected)
                return false;

            return true;
        }

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKey), typeof(KeyCode))]
            internal static bool GetKeyCode() => HotkeyBlock();
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKey), typeof(string))]
            internal static bool GetKeyString() => HotkeyBlock();
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(KeyCode))]
            internal static bool GetKeyDownCode() => HotkeyBlock();
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(string))]
            internal static bool GetKeyDownString() => HotkeyBlock();
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(KeyCode))]
            internal static bool GetKeyUpCode() => HotkeyBlock();
            [HarmonyPrefix, HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(string))]
            internal static bool GetKeyUpString() => HotkeyBlock();
        }
    }
}
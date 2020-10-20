using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace COM3D2API
{
    /// <summary>
    /// API for adding buttons to the SystemShortcut menu (gear icon)
    /// </summary>
    public static class SystemShortcutAPI
    {
        private static bool DidSystemShortcutHook;
        private static readonly List<ButtonData> ButtonsToCreate = new List<ButtonData>();
        private static SystemShortcut SystemShortcutInstance;
        private static GameObject ConfigButton;
        private static UIGrid UIShortcutGrid;
        private static UILabel TooltipLabel;
        private static UISprite TooltipSprite;

        internal static void RegisterHooks() => Harmony.CreateAndPatchAll(typeof(Hooks));

        /// <summary>
        /// Add a button to the SystemShortcut menu
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="onClickEvent">Event to trigger when button is clicked</param>
        /// <param name="tooltipText">Text to show on mouse over</param>
        /// <param name="textureBytes">byte array containing image data for the icon</param>
        public static void AddButton(string name, Action onClickEvent, string tooltipText, byte[] textureBytes = null)
        {
            //If the SystemShortcut.Awake hook has already run add the button immediately, otherwise save the data which will run when the hook does
            if (DidSystemShortcutHook)
                CreateButton(name, onClickEvent, tooltipText, textureBytes);
            else
                ButtonsToCreate.Add(new ButtonData(name, onClickEvent, tooltipText, textureBytes));
        }

        /// <summary>
        /// Create a copy of a SystemShortcut button and wire it up
        /// </summary>
        private static void CreateButton(string name, Action onClickEvent, string tooltipText, byte[] textureBytes)
        {
            try
            {
                //Duplicate the config button
                GameObject buttonCopy = Object.Instantiate(ConfigButton, UIShortcutGrid.transform, true);
                buttonCopy.name = name;

                //Replace the onClick event
                var button = buttonCopy.GetComponent<UIButton>();
                button.onClick.Clear();
                EventDelegate.Add(button.onClick, () => onClickEvent());

                //Replace the UIEventTrigger events
                UIEventTrigger uiEventTrigger = buttonCopy.GetComponent<UIEventTrigger>();
                uiEventTrigger.onHoverOver.Clear();
                uiEventTrigger.onHoverOut.Clear();
                uiEventTrigger.onDragStart.Clear();
                EventDelegate.Add(uiEventTrigger.onHoverOver, () => ShowTooltip(tooltipText));
                EventDelegate.Add(uiEventTrigger.onHoverOut, HideTooltip);
                EventDelegate.Add(uiEventTrigger.onDragStart, HideTooltip);

                //Add the icon
                if (textureBytes != null)
                {
                    var tex = new Texture2D(32, 32, TextureFormat.BC7, false);
                    tex.LoadImage(textureBytes);

                    //Hide the original sprite
                    UISprite uiSprite = buttonCopy.GetComponent<UISprite>();
                    uiSprite.type = UIBasicSprite.Type.Filled;
                    uiSprite.fillAmount = 0.0f;

                    //Add the texture
                    UITexture uiTexture = NGUITools.AddWidget<UITexture>(buttonCopy);
                    uiTexture.material = new Material(uiTexture.shader);
                    uiTexture.material.mainTexture = tex;
                    uiTexture.MakePixelPerfect();
                }

                UIShortcutGrid.Reposition();
            }
            catch (Exception ex)
            {
                //Catch and show the error manually so errors in a single plugin don't break all the others
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Custom tooltip method because SystemShortcut.VisibleExplanation sends the text through LocalizationManager.GetTranslation and returns nothing if not found, apparently
        /// </summary>
        /// <param name="text">Tooltip text</param>
        private static void ShowTooltip(string text)
        {
            TooltipLabel.text = text;
            TooltipLabel.width = 0;
            TooltipLabel.MakePixelPerfect();
            TooltipSprite.width = TooltipLabel.width + 15;
            TooltipSprite.gameObject.SetActive(true);
        }
        /// <summary>
        /// Hide the tooltip
        /// </summary>
        private static void HideTooltip() => TooltipSprite.gameObject.SetActive(false);

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(SystemShortcut), "Awake")]
            private static void SystemShortcut_Awake(SystemShortcut __instance)
            {
                SystemShortcutInstance = __instance;

                try
                {
                    ConfigButton = SystemShortcutInstance.transform.Find("Base/Grid/Config").gameObject;
                    UIShortcutGrid = SystemShortcutInstance.GetComponentInChildren<UIGrid>();
                    TooltipLabel = (UILabel)Traverse.Create(SystemShortcutInstance).Field("m_labelExplanation").GetValue();
                    TooltipSprite = (UISprite)Traverse.Create(SystemShortcutInstance).Field("m_spriteExplanation").GetValue();

                    foreach (var button in ButtonsToCreate)
                        CreateButton(button.Name, button.OnClickEvent, button.TooltipText, button.TextureBytes);
                    ButtonsToCreate.Clear();

                    DidSystemShortcutHook = true;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        internal class ButtonData
        {
            public readonly string Name;
            public readonly Action OnClickEvent;
            public readonly string TooltipText;
            public readonly byte[] TextureBytes;

            public ButtonData(string name, Action onClickEvent, string tooltipText, byte[] textureBytes)
            {
                Name = name;
                OnClickEvent = onClickEvent;
                TooltipText = tooltipText;
                TextureBytes = textureBytes;
            }
        }
    }
}

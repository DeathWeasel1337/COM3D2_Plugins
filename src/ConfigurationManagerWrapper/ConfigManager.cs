using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace COM3D2_Plugins
{
    [BepInDependency(ConfigurationManager.ConfigurationManager.GUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ConfigurationManagerWrapper : BaseUnityPlugin
    {
        public const string PluginGUID = "deathweasel.com3d2.configurationmanagerwrapper";
        public const string PluginName = "Configuration Manager Wrapper for COM3D2";
        public const string PluginVersion = "1.0";
        private static ConfigurationManager.ConfigurationManager _manager;

        private void Awake() => Harmony.CreateAndPatchAll(typeof(Hooks));

        private void Start()
        {
            _manager = GetComponent<ConfigurationManager.ConfigurationManager>();

            //Disable the ConfigurationManager hotkey since we're adding a button to show the UI instead
            _manager.OverrideHotkey = true;
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(SystemShortcut), "Awake")]
            private static void SystemShortcut_Awake(SystemShortcut __instance)
            {
                GameObject configButton = __instance.transform.Find("Base/Grid/Config").gameObject;
                UIGrid uiShortcutGrid = __instance.GetComponentInChildren<UIGrid>();
                var m_labelExplanation = (UILabel)Traverse.Create(__instance).Field("m_labelExplanation").GetValue();
                var m_spriteExplanation = (UISprite)Traverse.Create(__instance).Field("m_spriteExplanation").GetValue();

                //Duplicate the config button
                GameObject configManagerButton = Instantiate(configButton, uiShortcutGrid.transform, true);

                //Replace the onClick event
                var button = configManagerButton.GetComponent<UIButton>();
                button.onClick.Clear();
                EventDelegate.Add(button.onClick, () => { _manager.DisplayingWindow = !_manager.DisplayingWindow; });

                //Replace the UIEventTrigger events
                UIEventTrigger uiEventTrigger = configManagerButton.GetComponent<UIEventTrigger>();
                uiEventTrigger.onHoverOver.Clear();
                uiEventTrigger.onHoverOut.Clear();
                uiEventTrigger.onDragStart.Clear();
                EventDelegate.Add(uiEventTrigger.onHoverOver, () => ShowTooltip("BepInEx Configuration Manager"));
                EventDelegate.Add(uiEventTrigger.onHoverOut, HideTooltip);
                EventDelegate.Add(uiEventTrigger.onDragStart, HideTooltip);

                //Custom tooltip method because SystemShortcut.VisibleExplanation sends the text through LocalizationManager.GetTranslation and returns nothing if not found, apparently
                void ShowTooltip(string text)
                {
                    m_labelExplanation.text = text;
                    m_labelExplanation.width = 0;
                    m_labelExplanation.MakePixelPerfect();
                    UISprite component = m_spriteExplanation.GetComponent<UISprite>();
                    component.width = m_labelExplanation.width + 15;
                    m_spriteExplanation.gameObject.SetActive(true);
                }
                void HideTooltip() => m_spriteExplanation.gameObject.SetActive(false);

                uiShortcutGrid.Reposition();
            }
        }
    }
}
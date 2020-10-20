using BepInEx;
using BepInEx.Logging;

namespace COM3D2API
{
    /// <summary>
    /// Contains information about the plugin itself
    /// </summary>
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// GUID of the plugin, use with <see cref="BepInDependency"/>:
        /// <code>[BepInDependency(COM3D2API.Plugin.PluginGUID)]</code>
        /// </summary>
        public const string PluginGUID = "deathweasel.com3d2.api";
        /// <summary> Name of the plugin </summary>
        public const string PluginName = "COM3D2 API";
        /// <summary> Version of the plugin </summary>
        public const string PluginVersion = "1.0";
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;

            SystemShortcutAPI.RegisterHooks();
        }
    }
}

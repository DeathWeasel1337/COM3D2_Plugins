using BepInEx;
using BepInEx.Logging;
using COM3D2API;
using System.IO;
using System.Reflection;

[BepInDependency(ConfigurationManager.ConfigurationManager.GUID)]
[BepInDependency(Plugin.PluginGUID)]
[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class ConfigurationManagerWrapper : BaseUnityPlugin
{
    public const string PluginGUID = "deathweasel.com3d2.configurationmanagerwrapper";
    public const string PluginName = "Configuration Manager Wrapper for COM3D2";
    public const string PluginVersion = "1.0";
    internal static new ManualLogSource Logger;
    private static ConfigurationManager.ConfigurationManager ConfigManagerInstance;

    private void Start()
    {
        Logger = base.Logger;
        ConfigManagerInstance = GetComponent<ConfigurationManager.ConfigurationManager>();

        SystemShortcutAPI.AddButton("ConfigManager", () => ConfigManagerInstance.DisplayingWindow = !ConfigManagerInstance.DisplayingWindow, "BepInEx Configuration Manager", LoadIcon());

        //Disable the ConfigurationManager hotkey since we're adding a button to show the UI instead
        ConfigManagerInstance.OverrideHotkey = true;
    }

    private static byte[] LoadIcon()
    {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("COM3D2_Plugins.Resources.icon.png"))
            if (stream != null)
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                return bytesInStream;
            }
        return null;
    }
}

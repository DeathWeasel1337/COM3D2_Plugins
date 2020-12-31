# COM3D2_Plugins
Plugins for Custom Order Maid 3D2

## Installation
1. Install [BepInEx v5.3](https://github.com/BepInEx/BepInEx/releases)
2. Extract the plugin .zip file to your game folder

The plugin .dll should end up in COM3D2\BepInEx\plugins

#### COM3D2 API
**v1.0 - [Download](https://github.com/DeathWeasel1337/COM3D2_Plugins/releases/download/v3/COM3D2.API.v1.0.zip)**

API plugins can use to do stuff. Currently only has an API for adding buttons to the SystemShortcut (gear icon) menu.

#### ConfigurationManagerWrapper
**v1.0 - [Download](https://github.com/DeathWeasel1337/COM3D2_Plugins/releases/download/v3/COM3D2.ConfigurationManagerWrapper.v1.0.zip)**

Adds a button to the menu which opens the [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager) window. Requires COM3D2 API.

#### InputHotkeyBlock
**v1.2 - [Download](https://github.com/DeathWeasel1337/COM3D2_Plugins/releases/download/v5/COM3D2.InputHotkeyBlock.v1.2.zip)**

Prevents hotkeys from plugins from triggering while typing in input fields.

Because input fields don't ever seem to lose focus on their own, input fields are forced to lose focus when clicking anywhere outside of them. When no input field is focused hotkeys will function as normal.

<details><summary>Change Log</summary>
v1.1 Support for Unity InputFields<br/>
v1.2 No long blocks return or backspace<br/>
</details>

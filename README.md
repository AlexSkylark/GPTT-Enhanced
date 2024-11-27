# Gradual Progression Tech Tree Organizer (GPTT-Organizer)

**GPTT-Organizer** is an addon mod for **[Gradual Progression Tech Tree (GPTT)](https://github.com/Author/GPTT)**, designed to improve the organization and visual layout of the tech tree in **Kerbal Space Program (KSP)**. This mod requires GPTT to function but adds its own unique features.

---

## Features

### 1. Enhanced Lost and Found Node
- Adds grouping and headers to the **Lost and Found** node.
- Automatically organizes parts in the node based on their originating mod.
- Makes it easier to locate unsupported parts added by other mods.

### 2. Configurable Node Adjustments
- Introduces a user-configurable option to reposition certain nodes in the center of the tech tree.
- Allows players to adjust the layout for a cleaner, more intuitive view.
- Changes take effect on game start after the configuration is updated.

---

## Installation

### Dependencies
- **[Gradual Progression Tech Tree (GPTT)](https://github.com/Author/GPTT)**
- **[Module Manager](https://github.com/sarbian/ModuleManager)** (included in most mod packs).

### Steps
1. Download the latest release of GPTT-Organizer from the [Releases Page](https://github.com/AlexSkylark/GPTT-Organizer/releases).
2. Extract the contents of the archive.
3. Place the `GameData/GPTT-Organizer` folder into your KSP `GameData` directory.

## Configuration Options

You can enable or disable tech tree layout adjustments via a user settings file:

1. Locate the file:
   ```
   GameData/GPTT-Organizer/UserSettings.cfg
   ```

2. Edit the file to configure the layout adjustment:
   ```
   NodeAdjustments
   {
       AdjustTechTreeNodes = true
   }
   ```

   - Set `AdjustTechTreeNodes` to `true` or `false` as desired.
   - Changes will take effect after starting the game.

---

## Compatibility

- Tested with **KSP 1.12.x**.
- Should work with most mods unless they directly alter the same tech tree nodes.

---

## License

This mod is licensed under the **Creative Commons Attribution-NonCommercial 4.0 International License (CC BY-NC 4.0)**. Refer to the `LICENSE.md` file for details.

---

## Support

For issues, questions, or feedback, please visit the mod's GitHub repository:
[GitHub Repository](https://github.com/AlexSkylark/GPTT-Organizer)

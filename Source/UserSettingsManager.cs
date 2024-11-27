using System.IO;
using UnityEngine;

namespace GPTT
{ 
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class UserSettingsManager : MonoBehaviour
    {
        void Start()
        {
            // Write a dynamic Module Manager config
            WriteDynamicConfig(ReadUserSetting());
        }

        private bool ReadUserSetting()
        {
            string path = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Organizer/UserSettings.cfg");
            if (File.Exists(path))
            {
                ConfigNode settingsNode = ConfigNode.Load(path);
                if (settingsNode != null && settingsNode.HasNode("GPTTOrganizerSetting"))
                {
                    return bool.TryParse(settingsNode.GetNode("GPTTOrganizerSetting").GetValue(""), out bool value) && value;
                }
            }
            return false; // Default to false
        }

        private void WriteDynamicConfig(bool enabled)
        {
            if (enabled)
            {
                if (File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Organizer/NodeAdjustments.cfg")))
                    File.Move(Path.Combine(
                            KSPUtil.ApplicationRootPath,
                            "GameData/GPTT-Organizer/NodeAdjustments.cfg.OFF"),
                        Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Organizer/NodeAdjustments.cfg"));
            } else
            {
                if (File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Organizer/NodeAdjustments.cfg")))
                    File.Move(Path.Combine(
                            KSPUtil.ApplicationRootPath,
                            "GameData/GPTT-Organizer/NodeAdjustments.cfg"),
                        Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Organizer/NodeAdjustments.cfg.OFF"));
            }
        }
    }
}

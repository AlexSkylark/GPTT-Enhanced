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
            string path = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/UserSettings.cfg");
            if (File.Exists(path))
            {
                ConfigNode settingsNode = ConfigNode.Load(path);
                if (settingsNode != null && settingsNode.HasNode("GPTTEnhancedSetting"))
                {
                    return bool.TryParse(settingsNode.GetNode("GPTTEnhancedSetting").GetValue(""), out bool value) && value;
                }
            }
            return false; // Default to false
        }

        private void WriteDynamicConfig(bool enabled)
        {
            if (enabled)
            {
                if (File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/NodeAdjustments.cfg")))
                    File.Move(Path.Combine(
                            KSPUtil.ApplicationRootPath,
                            "GameData/GPTT-Enhanced/NodeAdjustments.cfg.OFF"),
                        Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/NodeAdjustments.cfg"));
            } else
            {
                if (File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/NodeAdjustments.cfg")))
                    File.Move(Path.Combine(
                            KSPUtil.ApplicationRootPath,
                            "GameData/GPTT-Enhanced/NodeAdjustments.cfg"),
                        Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/NodeAdjustments.cfg.OFF"));
            }
        }
    }
}

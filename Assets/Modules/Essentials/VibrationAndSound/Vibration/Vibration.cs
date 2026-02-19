using System;
// using Lofelt.NiceVibrations;
using Pixelplacement;
using UnityEngine;

namespace Setting
{
    public class Vibration : Singleton<Vibration>
    {
        private SettingData settingData;
        private SettingValue vibrationSetting;

        private void Awake()
        {
            settingData = DataController.Instance.GameData.SettingData;
            vibrationSetting = settingData.GetSettingByType(SettingType.Vibration);
        }

        public void VibrateLight()
        {
            if (!vibrationSetting.value) return;
            // HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }

        public void VibrateMedium()
        {
            if (!vibrationSetting.value) return;
            // HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        }

        public void VibrateHeavy()
        {
            if (!vibrationSetting.value) return;
            // HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        }
    }
}
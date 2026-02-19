using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum SettingType
{
    Music = 1,
    Sound = 2,
    Vibration = 3
}

[Serializable]
public class SettingValue
{
    public int id;
    public bool value;

    public SettingValue(int id, bool value)
    {
        this.id = id;
        this.value = value;
    }
}

[Serializable]
public class SettingData
{
    [SerializeField] private List<SettingValue> settingValues;

    public SettingData()
    {
        settingValues = new List<SettingValue>
        {
            new SettingValue((int)SettingType.Music, true),
            new SettingValue((int)SettingType.Sound, true),
            new SettingValue((int)SettingType.Vibration, true)
        };
    }

    public SettingValue GetSettingById(int id)
    {
        foreach (var s in settingValues)
            if (s.id == id)
                return s;

        var settingValue = new SettingValue(id, true);
        settingValues.Add(settingValue);
        return settingValue;
    }

    public SettingValue GetSettingByType(SettingType type)
    {
        return GetSettingById((int)type);
    }
}
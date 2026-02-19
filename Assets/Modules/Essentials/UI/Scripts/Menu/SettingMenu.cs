using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Setting;
using UnityEngine.SceneManagement;

public class SettingMenu : MenuBase
{
    [SerializeField] protected Toggle bgMusicToggle;
    [SerializeField] protected Toggle sfxToggle;
    [SerializeField] protected Toggle vibrationToggle;
    [SerializeField] protected TMP_Text version;
    [SerializeField] protected TMP_InputField deviceId;
    protected SettingData settingData;
    private int count;

    protected void Awake()
    {
        settingData = DataController.Instance.GameData.SettingData;
        UpdateVersionAndDeviceId();
        
        PresetToggle(bgMusicToggle, SettingType.Music);
        PresetToggle(sfxToggle, SettingType.Sound);
        PresetToggle(vibrationToggle, SettingType.Vibration);
    }

    protected void UpdateVersionAndDeviceId()
    {
        version.SetText(Application.version);
        deviceId.SetTextWithoutNotify(SystemInfo.deviceUniqueIdentifier);
    }

    public void UpdateCount()
    {
        count++;
        deviceId.gameObject.SetActive(count > 10);
    }

    public void ToggleBGMusic(bool value)
    {
        UpdateToggle(value, SettingType.Music);
        SoundController.Instance.UpdateBackgroundMusicGroup();
    }
    
    public void ToggleSfx(bool value)
    {
        UpdateToggle(value, SettingType.Sound);
        SoundController.Instance.UpdateSfxGroup();
    }
    
    public void ToggleVibration(bool value)
    {
        UpdateToggle(value, SettingType.Vibration);
    }
    
    protected void UpdateToggle(bool value, SettingType settingType)
    {
        var s = settingData.GetSettingByType(settingType);
        s.value = value;
        DataController.Instance.SaveData(this);
    }

    private void PresetToggle(Toggle toggle, SettingType settingType)
    {
        var s = settingData.GetSettingByType(settingType);
        toggle.SetIsOnWithoutNotify(s.value);
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
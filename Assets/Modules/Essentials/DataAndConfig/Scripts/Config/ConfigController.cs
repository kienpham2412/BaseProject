using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Pixelplacement;
using UnityEngine;

public class ConfigController : Singleton<ConfigController>
{
    public GameplayConfig GameplayConfig { get; private set; }
    public TestDevices TestDevices { get; private set; }
    public AdConfig AdConfig { get; private set; }
    public IapConfig IapConfig { get; private set; }
    public TutorialConfig TutorialConfig { get; private set; }
    public UpdateConfig UpdateConfig { get; private set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        GameplayConfig = JsonUtility.FromJson<GameplayConfig>(GetLocalConfig("gameplay_config"));
        IapConfig = JsonUtility.FromJson<IapConfig>(GetLocalConfig("iap_config"));
        AdConfig = JsonUtility.FromJson<AdConfig>(GetLocalConfig("ad_config"));
        TestDevices = JsonUtility.FromJson<TestDevices>(GetRemoteConfig("test_device"));
        TutorialConfig = JsonUtility.FromJson<TutorialConfig>(GetLocalConfig("tutorial_config"));
        UpdateConfig = JsonUtility.FromJson<UpdateConfig>(GetRemoteConfig("update_config"));
    }

    private void Start()
    {
        SetDebugLogger();
    }

    private string GetRemoteConfig(string fileName)
    {
        return FirebaseServiceController.Instance.GetConfig(fileName);
        // return GetLocalConfig(fileName);
    }

    private string GetLocalConfig(string fileName, string resourceLocation = "Json/")
    {
        var localPath = resourceLocation + fileName;
        return Resources.Load<TextAsset>(localPath).text;
    }

    public void SetDebugLogger()
    {
#if !UNITY_EDITOR
        DebugLogger.enable = TestDevices.Contain(SystemInfo.deviceUniqueIdentifier);
#else
        DebugLogger.enable = true;
#endif
    }

    public bool CanCheat()
    {
#if UNITY_EDITOR
        return true;
#else
        return TestDevices.Contain(SystemInfo.deviceUniqueIdentifier);
#endif
    }
}

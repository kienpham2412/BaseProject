using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Pixelplacement;
using Path = System.IO.Path;

[RequireComponent(typeof(Initialization))]
public class DataController : Singleton<DataController>
{
    private string KEY = "FitJam";
    private string dataPath = "";
    [field: SerializeField] public GameData GameData { get; private set; }
    public bool DataLoaded { get; private set; } = false;
    public bool FirstTimeOpened { get; private set; } = false;

    protected override void OnRegistration()
    {
        Debug.Log("Device id " + SystemInfo.deviceUniqueIdentifier);
        Debug.Log($"Aspect ratio: {(float)Screen.width / (float)Screen.height}");
        dataPath = Path.Combine(Application.persistentDataPath, "data.dat");

#if !UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
#endif
    }

    public void LoadData()
    {
        if (File.Exists(dataPath))
        {
            Debug.Log("file exist");
            try
            {
                string data = File.ReadAllText(dataPath);
                string decrypted = XOROperator(data, KEY);
                GameData = JsonUtility.FromJson<GameData>(decrypted);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                ResetData();
            }
        }
        else
            ResetData();

        DataLoaded = true;
        // DebugLogger.Log("Load data from " + dataPath, Color.green);
    }

    public void ResetData()
    {
        FirstTimeOpened = true;
        GameData = new GameData();
    }

    public void SaveData(object invoker = null)
    {
        string origin = JsonUtility.ToJson(GameData);
        string encrypted = XOROperator(origin, KEY);
        File.WriteAllText(dataPath, encrypted);

        if (invoker != null) DebugLogger.Log($"Save {this.GetType()} data invoked by {invoker.GetType()}", Color.green);
    }

    private void Awake()
    {
        LoadData();
    }

    public static string XOROperator(string input, string key)
    {
        char[] output = new char[input.Length];
        for (int i = 0; i < input.Length; i++)
            output[i] = (char)(input[i] ^ key[i % key.Length]);

        return new string(output);
    }
}

[System.Serializable]
public class GameData
{
    [SerializeField] private Collectibles collectibles;
    public Collectibles Collectibles => collectibles ??= new Collectibles();

    [SerializeField] private GameplayData gameplayData;
    public GameplayData GameplayData => gameplayData ??= new GameplayData();
    
    [SerializeField] private SettingData settingData;
    public SettingData SettingData => settingData ??= new SettingData();
    
    [SerializeField] private TutorialData tutorialData;
    public TutorialData TutorialData => tutorialData ??= new TutorialData();
    
    [SerializeField] private LevelData levelData;
    public LevelData LevelData => levelData ??= new LevelData();
}
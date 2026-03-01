using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.Events;
// using Firebase.Extensions;
// using Firebase.RemoteConfig;
// using Firebase.Crashlytics;

[RequireComponent(typeof(Initialization))]
public class FirebaseServiceController : Singleton<FirebaseServiceController>
{
    public bool isTestMode;
    public bool IsFirebaseInited { get; private set; }
    
    protected override void OnRegistration()
    {
#if !UNITY_EDITOR
            isTestMode=false;
#endif
        // Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
        // Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        // {
        //     dependencyStatus = task.Result;
        //     if (dependencyStatus == Firebase.DependencyStatus.Available)
        //     {
        //         System.Threading.Tasks.Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero);
        //         fetchTask.ContinueWithOnMainThread(OnFetchCompleted);
        //         Crashlytics.IsCrashlyticsCollectionEnabled = true;
        //         Debug.Log("Firebase initialized successfully!");
        //     }
        //     else
        //     {
        //         Debug.LogError("Firebase: Could not resolve all Firebase dependencies: " + dependencyStatus);
        //     }
        //     IsFirebaseInited = true;
        // });
        
        
        IsFirebaseInited = true; // comment this line if firebase package is imported
    }

    // private void OnFetchCompleted(System.Threading.Tasks.Task fetchTask)
    // {
    //     if (fetchTask.IsFaulted)
    //     {
    //         Debug.LogError("Firebase: cant fetch remote config");
    //         Debug.Log(fetchTask.Exception);
    //     }
    //     else if (fetchTask.IsCompleted)
    //     {
    //         FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
    //     }
    //     IsFirebaseInited = true;
    // }

    public string GetConfig(string configName, string defaultPath = "Json/")
    {
        bool success;
        return GetConfig(configName, out success, defaultPath);
    }

    public string GetConfig(string configName, out bool isSuccess, string defaultPath = "Json/")
    {
        string result = null;
        // if (!isTestMode) result = FirebaseRemoteConfig.DefaultInstance.GetValue(configName).StringValue;
        if (result == null || result == "")
        {
            Debug.Log($"Firebase: Fail to load {configName} from firebase");
            isSuccess = false;
            result = Resources.Load<TextAsset>(defaultPath + configName).text;
        }
        else
        {
            isSuccess = true;
            Debug.Log($"Firebase: Load {configName} from firebase successfully");
        }
        return result;
    }
}


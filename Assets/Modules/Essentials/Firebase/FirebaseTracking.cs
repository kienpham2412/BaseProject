using System;
// using Firebase.Analytics;
using Pixelplacement;
using UnityEngine;

public class FirebaseTracking : Singleton<FirebaseTracking>
{
    private LevelData levelData;

    private void Awake()
    {
        levelData = DataController.Instance.GameData.LevelData;
    }
}

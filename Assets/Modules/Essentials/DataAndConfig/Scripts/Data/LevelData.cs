using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LevelCompletionInfo
{
    public int numberOfStars;

    public LevelCompletionInfo()
    {
        numberOfStars = 0;
    }
}

[Serializable]
public class LevelData
{
    [SerializeField] private List<LevelCompletionInfo> levelCompletionInfos;
    public int lastestUnlockedLevel;
    public int CurrentLevel { get; set; } = 1;

    public LevelData()
    {
        levelCompletionInfos = new List<LevelCompletionInfo>(300);
        lastestUnlockedLevel = 1;
    }

    public LevelCompletionInfo GetLevelCompletionInfo(int level)
    {
        var totalInfoCount = levelCompletionInfos.Count;
        var idx = level - 1;

        if (idx >= totalInfoCount)
        {
            var count = totalInfoCount;
            for (int i = totalInfoCount; i <= idx; i++)
            {
                var newInfo = new LevelCompletionInfo();
                levelCompletionInfos.Add(newInfo);
            }
        }

        return levelCompletionInfos[idx];
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LevelData
{
    public int level;

    public LevelData()
    {
        level = 1;
    }
}

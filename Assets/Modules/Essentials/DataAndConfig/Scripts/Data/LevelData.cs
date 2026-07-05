using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int level;
    public int boardId;

    public LevelData()
    {
        boardId = 1;
        level = 1;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestDevices
{
    [SerializeField] private List<string> testIds;

    public bool Contain(string deviceId)
    {
        return testIds.Contains(deviceId);
    }
}
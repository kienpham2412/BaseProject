using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ClearData
{
    [MenuItem("Tools/Clear Data")]
    static void Clear()
    {
        PlayerPrefs.DeleteAll();
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
        foreach (string filePath in filePaths)
            if (filePath.Contains(".dat"))
                File.Delete(filePath);
    }

}

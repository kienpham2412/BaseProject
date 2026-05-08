// ServiceAccountTool.cs
// Đặt file trong thư mục Editor

using System.IO;
using UnityEditor;
using UnityEngine;

public class ServiceAccountTool : EditorWindow
{
    private enum Tab
    {
        Decript,
        Encript
    }

    private const string OUTPUT_PATH =
        "Assets/Modules/Essentials/GitHubWorkFlow/ServiceAccount/ServiceAccountEncripted.txt";

    private Tab currentTab;

    // Encript
    private string encryptPassword = "";
    private string encryptContent = "";

    // Decript
    private string decryptPassword = "";
    private string decryptResult = "";
    private Vector2 decryptScrollPosition;

    [MenuItem("Tools/Service Account Tool")]
    public static void ShowWindow()
    {
        ServiceAccountTool window = GetWindow<ServiceAccountTool>();

        window.titleContent = new GUIContent("Service Account Tool");
        window.minSize = new Vector2(600, 400);
    }

    private void OnGUI()
    {
        DrawToolbar();

        GUILayout.Space(10);

        switch (currentTab)
        {
            case Tab.Decript:
                DrawDecriptTab();
                break;

            case Tab.Encript:
                // DrawEncriptTab();
                break;
        }
    }

    private void DrawToolbar()
    {
        currentTab = (Tab)GUILayout.Toolbar(
            (int)currentTab,
            new string[]
            {
                "Decript",
                "Encript"
            },
            GUILayout.Height(30)
        );
    }

    private void DrawDecriptTab()
    {
        EditorGUILayout.LabelField("Decript", EditorStyles.boldLabel);

        GUILayout.Space(10);

        decryptPassword = EditorGUILayout.TextField(
            "Password",
            decryptPassword
        );

        GUILayout.Space(10);

        GUI.enabled = !string.IsNullOrEmpty(decryptPassword);

        if (GUILayout.Button("Decript", GUILayout.Height(40)))
        {
            DecriptFile();
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Result", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Copy", GUILayout.Width(60)))
        {
            EditorGUIUtility.systemCopyBuffer = decryptResult;

            Debug.Log("Copied decrypted text to clipboard.");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.SelectableLabel(
            decryptResult,
            EditorStyles.textArea,
            GUILayout.ExpandHeight(true)
        );
    }

    private void DrawEncriptTab()
    {
        EditorGUILayout.LabelField("Encript", EditorStyles.boldLabel);

        GUILayout.Space(10);

        encryptPassword = EditorGUILayout.TextField(
            "Password",
            encryptPassword
        );

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Content");

        encryptContent = EditorGUILayout.TextArea(
            encryptContent,
            GUILayout.ExpandHeight(true)
        );

        GUILayout.Space(10);

        GUI.enabled =
            !string.IsNullOrEmpty(encryptPassword) &&
            !string.IsNullOrEmpty(encryptContent);

        if (GUILayout.Button("Encript", GUILayout.Height(40)))
        {
            EncriptAndSave();
        }

        GUI.enabled = true;
    }

    private void EncriptAndSave()
    {
        string encrypted = XOROperator(encryptContent, encryptPassword);

        string directory = Path.GetDirectoryName(OUTPUT_PATH);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(OUTPUT_PATH, encrypted);

        AssetDatabase.Refresh();

        Debug.Log($"Encrypted file saved at: {OUTPUT_PATH}");
    }

    private void DecriptFile()
    {
        if (!File.Exists(OUTPUT_PATH))
        {
            Debug.LogError($"File not found: {OUTPUT_PATH}");
            decryptResult = "";
            return;
        }

        string encrypted = File.ReadAllText(OUTPUT_PATH);

        decryptResult = XOROperator(encrypted, decryptPassword);
    }

    public static string XOROperator(string input, string key)
    {
        char[] output = new char[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = (char)(input[i] ^ key[i % key.Length]);
        }

        return new string(output);
    }
}
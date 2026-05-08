using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PauseShortcut
{
    // Sets F7 as the shortcut to toggle pause
    [MenuItem("Tools/Toggle Pause _F1")]
    static void TogglePause()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
            Debug.Log(EditorApplication.isPaused ? "Paused" : "Resumed");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLogger
{
    public static bool enable = true;
    public static bool logEnable = true;
    public static bool logErrorEnable = true;
    public static bool logWarningEnable = true;

    public static void Log(string content)
    {
        if (enable && logEnable) Debug.Log(content);
    }

    public static void LogWarning(string content)
    {
        if (enable && logWarningEnable) Debug.LogWarning(content);
    }

    public static void LogError(string content)
    {
        if (enable && logErrorEnable) Debug.LogError(content);
    }

    public static void Log(string content, Color color)
    {
        if (enable && logEnable)
        {
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            string colorContent = $"<color=#{hexColor}>{content}</color>";
            Debug.Log(colorContent);
        }
    }
}
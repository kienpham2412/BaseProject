using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Client.Editor
{
    [InitializeOnLoad]
    public static class GameViewCaptureToolbar
    {
        private static ScriptableObject m_toolbar;

        static GameViewCaptureToolbar()
        {
            EditorApplication.update -= TryCreateContainer;
            EditorApplication.update += TryCreateContainer;
        }

        // ===== IMGUI =====
        private static void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);

            if (GUILayout.Button(EditorGUIUtility.IconContent("Camera Icon"),
                    GUILayout.Width(28), GUILayout.Height(22)))
            {
                CaptureGameView();
            }

            GUILayout.EndHorizontal();
        }

        // ===== CAPTURE LOGIC =====
        private static void CaptureGameView()
        {
            string folder = Path.Combine(Application.dataPath, "../Screenshots");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = $"GameView_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string path = Path.Combine(folder, fileName);

            ScreenCapture.CaptureScreenshot(path);
            Debug.Log($"📸 GameView captured: {path}");
        }

        // ===== TOOLBAR INJECTION =====
        private static void TryCreateContainer()
        {
            if (m_toolbar != null) return;

            VisualElement root = GetMainToolbarRoot();
            if (root == null) return;

            VisualElement toolbarZone = root.Q("ToolbarZonePlayMode");
            if (toolbarZone == null) return;

            VisualElement parent = new()
            {
                style =
                {
                    flexGrow = 0,
                    flexDirection = FlexDirection.Row,
                }
            };

            IMGUIContainer container = new();
            container.style.flexGrow = 0;
            container.onGUIHandler += OnGUI;

            parent.Add(container);
            toolbarZone.Add(parent);
        }

        // ===== REFLECTION HACK =====
        private static VisualElement GetMainToolbarRoot()
        {
            // UnityEditor.Toolbar (internal singleton)
            m_toolbar = Type.GetType("UnityEditor.Toolbar, UnityEditor")
                ?.GetField("get", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null) as ScriptableObject;

            if (m_toolbar == null) return null;

            // GUIView.windowBackend
            object windowBackend = Type.GetType("UnityEditor.GUIView, UnityEditor")
                ?.GetProperty("windowBackend", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(m_toolbar);

            if (windowBackend == null) return null;

            // IWindowBackend.visualTree
            VisualElement visualTree = Type.GetType("UnityEditor.IWindowBackend, UnityEditor")
                ?.GetProperty("visualTree")
                ?.GetValue(windowBackend) as VisualElement;

            return visualTree;
        }
    }
}

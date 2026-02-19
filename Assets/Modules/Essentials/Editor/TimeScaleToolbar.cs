using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Client.Editor
{
    [InitializeOnLoad]
    public static class TimeScaleToolbar
    {
        private const string KeyTimeScale = "TimeScaleToolbar_TimeScale";
        private const float MaxValue = 2f;

        private static float m_timeScale;
        private static ScriptableObject m_toolbar;

        static TimeScaleToolbar()
        {
            m_timeScale = Mathf.Min(EditorPrefs.GetFloat(KeyTimeScale, 1), MaxValue);
            EditorApplication.update -= TryCreateContainer;
            EditorApplication.update += TryCreateContainer;
        }

        private static void OnGUI()
        {
            GUILayout.BeginHorizontal();
            const float offset = 10f;
            GUILayout.Space(offset);
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(1));
            GUILayout.Label("TS", GUILayout.Width(20));
            const float sliderWidth = 150f;
            float oldScale = Time.timeScale;
            m_timeScale = GUILayout.HorizontalSlider(m_timeScale, 0, MaxValue, GUILayout.Width(sliderWidth));
            GUILayout.Space(4);
            GUILayout.Label(m_timeScale.ToString("F2"), GUILayout.Width(30f));
            if (GUILayout.Button("R", GUILayout.Width(20))) m_timeScale = 1;
            Time.timeScale = m_timeScale;
            GUILayout.EndHorizontal();
            if (Math.Abs(oldScale - m_timeScale) > 0.0001f) EditorPrefs.SetFloat(KeyTimeScale, m_timeScale);
            GUILayout.EndHorizontal();
        }

        private static void TryCreateContainer()
        {
            // return if already created
            if (m_toolbar != null) return;

            VisualElement root = GetMainToolbarRoot();
            if (root == null) return;

            VisualElement toolbarZone = root.Q("ToolbarZoneRightAlign");

            VisualElement parent = new()
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };
            IMGUIContainer container = new();
            container.style.flexGrow = 1;
            container.onGUIHandler += OnGUI;
            parent.Add(container);
            toolbarZone.Add(parent);
        }

        private static VisualElement GetMainToolbarRoot()
        {
            // Get Toolbar static instance
            m_toolbar = Type.GetType("UnityEditor.Toolbar, UnityEditor")
                ?.GetField("get", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as ScriptableObject;
            if (m_toolbar == null) return null;
            // Get Toolbar.windowBackend
            object windowBackend = Type.GetType("UnityEditor.GUIView, UnityEditor")
                ?.GetProperty("windowBackend", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(m_toolbar);
            if (windowBackend == null) return null;
            // Get Toolbar.windowBackend.visualTree
            VisualElement visualTree = (VisualElement) Type.GetType("UnityEditor.IWindowBackend, UnityEditor")
                ?.GetProperty("visualTree")?.GetValue(windowBackend);
            return visualTree;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Loading
{
    public class SceneNameAttribute : PropertyAttribute
    {
        public bool showScenePath; // Tùy chọn hiển thị đường dẫn Scene

        public SceneNameAttribute(bool showScenePath = false)
        {
            this.showScenePath = showScenePath;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneNameAttribute sceneAttribute = (SceneNameAttribute)attribute;

                // Lấy danh sách các Scene trong Build Settings
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                string[] sceneNames = new string[scenes.Length];
                int selectedIndex = -1;

                for (int i = 0; i < scenes.Length; i++)
                {
                    sceneNames[i] = Path.GetFileNameWithoutExtension(scenes[i].path);
                    if (property.stringValue == scenes[i].path)
                    {
                        selectedIndex = i;
                    }
                }

                // Hiển thị Popup chọn Scene
                int newSelectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames);

                if (newSelectedIndex != selectedIndex)
                {
                    if (newSelectedIndex >= 0 && newSelectedIndex < scenes.Length)
                    {
                        property.stringValue = scenes[newSelectedIndex].path;
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }

                if (sceneAttribute.showScenePath && !string.IsNullOrEmpty(property.stringValue))
                {
                    // Hiển thị đường dẫn nếu tùy chọn được bật
                    Rect pathRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(pathRect, "Path: " + property.stringValue);
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Sử dụng SceneAttribute cho biến kiểu string!");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SceneNameAttribute sceneAttribute = (SceneNameAttribute)attribute;
            return base.GetPropertyHeight(property, label) + (sceneAttribute.showScenePath && !string.IsNullOrEmpty(property.stringValue) ? EditorGUIUtility.singleLineHeight : 0);
        }
    }
#endif
}
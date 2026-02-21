using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine.Events;
#endif

namespace Essential
{
    [Serializable]
    public class Transition
    {
        public AnimationCurve easeCurve;
        public float duration;
    }

    [Serializable]
    public class TransitionSpeed
    {
        public AnimationCurve easeCurve;
        public float speed;
    }

    [Serializable]
    public class Vector3Duration
    {
        public Vector3 endValue;
        public float duration;
    }

    [Serializable]
    public class Vector3Transition : Transition
    {
        public Vector3 endValue;
    }

    [Serializable]
    public class Vector3TransitionArray : Transition
    {
        public Vector3[] endValues;
    }

    [Serializable]
    public class ColorTransition : Transition
    {
        public Color endValue;
    }

    [Serializable]
    public class FloatTransition : Transition
    {
        public float endValue;
    }

    [Serializable]
    public class FoundationStyle
    {
        public Material fontMaterial;
        public Color color;
        public Color cardTextColor;
    }

    [CreateAssetMenu(fileName = "FeelingSetting", menuName = "Feeling Setting")]
    public class FeelingSetting : ScriptableObject
    {
        private static FeelingSetting cachedInstance;

        public static FeelingSetting Load()
        {
            cachedInstance ??= LoadDirectlyFromResources();
            return cachedInstance;
        }

        public static FeelingSetting LoadDirectlyFromResources()
        {
            return Resources.Load<FeelingSetting>("ScriptableAsset/FeelingSetting");
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FadeGroupProperty : Attribute
    {
        public string groupName;

        public FadeGroupProperty(string groupName)
        {
            this.groupName = groupName;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FeelingSetting))]
    public class FeelingSettingEditor : Editor
    {
        private FeelingSetting feelingSetting;
        private SerializedObject feelingSettingSobj;
        private Dictionary<string, FeelingSettingFadeGroup> fadeGroups;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            feelingSetting = (FeelingSetting)target;
            feelingSettingSobj = new SerializedObject(feelingSetting);

            fadeGroups = new Dictionary<string, FeelingSettingFadeGroup>();
            FindFields();
        }

        public override void OnInspectorGUI()
        {
            feelingSettingSobj.Update();
            Draw();
            feelingSettingSobj.ApplyModifiedProperties();
        }

        private void Draw()
        {
            foreach (KeyValuePair<string, FeelingSettingFadeGroup> g in fadeGroups)
            {
                g.Value.Draw();
            }
        }

        private void FindFields()
        {
            Type type = target.GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (var f in fields)
            {
                if (Attribute.IsDefined(f, typeof(FadeGroupProperty)))
                {
                    var attributes = f.GetCustomAttribute<FadeGroupProperty>();
                    AddPropertyToGroup(f.Name, attributes.groupName);
                }
            }
        }

        private void AddPropertyToGroup(string fieldName, string attributeName)
        {
            var property = feelingSettingSobj.FindProperty(fieldName);
            var group = GetGroup(attributeName);
            group.AddProperty(property);
        }

        private FeelingSettingFadeGroup GetGroup(string groupName)
        {
            if (!fadeGroups.ContainsKey(groupName))
                fadeGroups.Add(groupName, new FeelingSettingFadeGroup(groupName));

            return fadeGroups[groupName];
        }
    }

    public class FeelingSettingFadeGroup
    {
        public List<SerializedProperty> properties;
        private FadeGroup fadeGroup;

        public FeelingSettingFadeGroup(string title)
        {
            properties = new List<SerializedProperty>(10);
            fadeGroup = new FadeGroup(title, DrawContent);
        }

        public void AddProperty(SerializedProperty property)
        {
            properties.Add(property);
        }

        private void DrawContent()
        {
            foreach (var p in properties)
            {
                EditorGUILayout.PropertyField(p);
            }
        }

        public void Draw()
        {
            fadeGroup.Draw();
        }
    }

    public class FadeGroup
    {
        private string title;
        private UnityAction Content;
        private static Dictionary<string, bool> cachedExpandsion;

        private static Dictionary<string, bool> CachedExpandsion
            => cachedExpandsion ??= new Dictionary<string, bool>(30);

        public FadeGroup(string title, UnityAction content)
        {
            this.title = title;
            this.Content = content;
        }

        public void Draw()
        {
            DrawContent(title, Content);
        }

        public static void DrawContent(string title, UnityAction Content)
        {
            if (!CachedExpandsion.ContainsKey(title))
                CachedExpandsion.Add(title, false);

            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button(title, EditorStyles.toolbarPopup)) CachedExpandsion[title] = !CachedExpandsion[title];
            if (CachedExpandsion[title]) Content?.Invoke();
            EditorGUILayout.EndVertical();
        }
    }
#endif
}
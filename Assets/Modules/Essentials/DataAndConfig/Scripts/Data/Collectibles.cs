using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum CollectibleType
{
    Custom = 0,
    RemoveAd = 1,
    Coin = 1000000,
    Gem = 2000000,
    Skin0 = 1000,
    Skin1 = 1001,
    Skin2 = 1002,
    Skin3 = 1003,
    Skin4 = 1004,
    Skin5 = 1005,
    Skin6 = 1006,
    Skin7 = 1007,
    Skin8 = 1008,
    Skin9 = 1009,
    Skin10 = 1010,
    Skin11 = 1011,
    Skin12 = 1012
}

[System.Serializable]
public class Collectible
{
    [SerializeField] protected CollectibleType id;
    [SerializeField] protected int amount;

    public CollectibleType Id
    {
        get => id;
    }

    public int Amount
    {
        get => amount;
        set => amount = value;
    }

    public Collectible(CollectibleType id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}

[System.Serializable]
public class Collectibles
{
    public List<Collectible> collectibles;

    public Collectibles()
    {
        collectibles = new List<Collectible>();
        collectibles.Add(new Collectible(CollectibleType.Skin0, 1));
    }

    public Collectible GetCollectibleById(CollectibleType id, int baseValue = 0)
    {
        foreach (var _collectible in collectibles)
            if (_collectible.Id == id)
                return _collectible;

        var collectible = new Collectible(id, baseValue);
        collectibles.Add(collectible);
        return collectible;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Collectible)), CanEditMultipleObjects]
public class CollectibleDrawer : PropertyDrawer
{
    private const string ID = "id";
    private const string AMOUNT = "amount";
    private const float INLINE_PADDING = 5f;
    private const float LABEL_TO_PROPERTY_RATIO = 0.4f;
    private const float NAME_TO_LABEL_RATIO = 0.6f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var id = property.FindPropertyRelative(ID);
        var amount = property.FindPropertyRelative(AMOUNT);

        var labelFieldWidth = position.width * LABEL_TO_PROPERTY_RATIO;
        var nameFieldWidth = labelFieldWidth * NAME_TO_LABEL_RATIO;
        var idFieldWidth = labelFieldWidth - nameFieldWidth - INLINE_PADDING;
        var intFieldWidth = position.width - labelFieldWidth - INLINE_PADDING;
        var choosen = CollectibleType.Custom;

        if (Enum.IsDefined(typeof(CollectibleType), id.intValue) && id.intValue != (int)CollectibleType.Custom)
        {
            id.intValue = (int)(CollectibleType)EditorGUI.EnumPopup(
                new Rect(position.x, position.y, labelFieldWidth, EditorGUIUtility.singleLineHeight),
                (CollectibleType)id.intValue);
        }
        else
        {
            choosen = (CollectibleType)EditorGUI.EnumPopup(
                new Rect(position.x, position.y, nameFieldWidth, EditorGUIUtility.singleLineHeight), choosen);
            if (choosen != CollectibleType.Custom) id.intValue = (int)choosen;
            else
                id.intValue = EditorGUI.IntField
                (new Rect(position.x + nameFieldWidth + INLINE_PADDING, position.y, idFieldWidth, EditorGUIUtility.singleLineHeight),
                    id.intValue);
        }

        amount.intValue = EditorGUI.IntField
        (new Rect(position.x + labelFieldWidth + INLINE_PADDING, position.y, intFieldWidth, EditorGUIUtility.singleLineHeight),
            amount.intValue);
    }
}
#endif
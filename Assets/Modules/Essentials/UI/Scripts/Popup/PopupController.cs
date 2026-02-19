using System;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class PopupController : Singleton<PopupController>
{
    [SerializeField] private PopupMenu[] popupMenus;

    private Dictionary<PopupType, bool> popupInstantiateFlag;

    private void Awake()
    {
        popupInstantiateFlag = new Dictionary<PopupType, bool>(10);
    }

    public PopupMenu GetPopup(PopupType type)
    {
        for (int i = 0; i < popupMenus.Length; i++)
        {
            var p = popupMenus[i];
            if (p.Type == type)
            {
                if (popupInstantiateFlag.ContainsKey(type) && popupInstantiateFlag[type]) return p;

                var instance = Instantiate(p, transform);
                instance.gameObject.SetActive(false);
                popupMenus[i] = instance;
                popupInstantiateFlag[type] = true;
                return instance;
            }
        }

        return null;
    }
}
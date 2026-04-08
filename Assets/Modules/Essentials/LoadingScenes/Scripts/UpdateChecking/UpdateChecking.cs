using System;
using System.Collections;
using UnityEngine;

public class UpdateChecking : MonoBehaviour
{
    [SerializeField] private PopupMenu updatePopup;
    private UpdateConfig updateConfig;
    public static bool UpdateCheckingComplete { get; private set; } = false;

    private void Awake()
    {
        updateConfig = ConfigController.Instance.UpdateConfig;
    }

    private void Start()
    {
        Version current = new Version(Application.version);
        Version lastest = new Version(updateConfig.lastestVersion);
        var versionMatch = current >= lastest;
        
        if (!updateConfig.enable || (updateConfig.enable && versionMatch))
        {
            UpdateCheckingComplete = true;
            return;
        }
        
        StartCoroutine(UpdateCheckingRoutine());
    }

    private IEnumerator UpdateCheckingRoutine()
    {
        updatePopup.TryGetMenu();
        var menu = (UpdateMenu) updatePopup.Menu;
        menu.ActivateContinueButton(!updateConfig.force);
        updatePopup.Display();
        
        yield return new WaitUntil(() => !updatePopup.gameObject.activeInHierarchy);
        yield return null;
        UpdateCheckingComplete = true;
    }
}

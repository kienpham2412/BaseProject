using UnityEngine;
using UnityEngine.UI;

public class UpdateMenu : MenuBase
{
    [SerializeField] private Button continueButton;
    private const string PACKAGE_NAME = "com.magato.jigsawsolitaire";
    
    public void GotoStore()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + PACKAGE_NAME);
#else
        Application.OpenURL("market://details?id=" + PACKAGE_NAME);
#endif
    }

    public void Continue()
    {
        Popup.Close();
    }

    public void ActivateContinueButton(bool isActive)
    {
        continueButton.gameObject.SetActive(isActive);
    }
}

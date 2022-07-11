using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelectorManager : MonoBehaviour
{
    public Sidemenu Sidemenu;

    public void ToggleOverlay(bool isShown, bool immediatelly = false)
    {
        Sidemenu.ToggleSideMenu(isShown, immediatelly);
    }

    public void SetLanguage(int localeIndex)
    {
        GlobalControl.Instance.UserSettings.SetLocale(localeIndex);
        Debug.Log("Selected language. closing");

        var hasDeeplink = !string.IsNullOrEmpty(Application.absoluteURL);

        GlobalControl.Instance.HandleDeepLinkOnStartup();
        ToggleOverlay(false, immediatelly: hasDeeplink);

        if (!hasDeeplink)
        {
            InterfaceInteraction.Instance.LoadStartupPage();
        }
    }
}

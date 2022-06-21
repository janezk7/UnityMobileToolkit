using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sample sidemenu manager
/// </summary>
public class MainAppSidemenuManager : MonoBehaviour
{
    private Sidemenu Sidemenu;

    public void Start()
    {
        Sidemenu = GetComponent<Sidemenu>();
        if (Sidemenu is null)
            Debug.LogError("Sidemenu manager script must be on Sidemenu prefab!");
    }

    /*
    // TODO: Implement manager logic
    */
    public void ToggleSideMenu()
    {
        bool isOpening = !Sidemenu.IsOpen;
        Sidemenu.ToggleSideMenu(isOpening);
    }

    public void NavigateToSettings()
    {
        InterfaceInteraction.Instance.NavigateToSettings();
        Sidemenu.ToggleSideMenu();
    }

    public void ShowLanguageSelector()
    {
        InterfaceInteraction.Instance.ToggleLanguageSelector(true);
        Sidemenu.ToggleSideMenu();
    }
}

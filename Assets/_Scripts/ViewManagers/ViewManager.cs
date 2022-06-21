using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A view must implement IView interface
/// </summary>
public class ViewManager: MonoBehaviour, IView
{
    public string Title = "Views and Navigation";
    public bool ShowMenuButtonInsteadOfBack = false;

    /* IView implementations */
    #region IView method implementations

    public virtual string GetTitle() => Title;
    public virtual void InitView()
    {
        // Implement your view initialization code
        if(ShowMenuButtonInsteadOfBack)
            InterfaceInteraction.Instance.ShowSidemenuButton();
        else
            InterfaceInteraction.Instance.ShowBackButton();
    }

    public virtual void NavigateBack()
    {
        // Implement your back functionality
        InterfaceInteraction.Instance.NavigatePreviousView();
    }

    #endregion
}

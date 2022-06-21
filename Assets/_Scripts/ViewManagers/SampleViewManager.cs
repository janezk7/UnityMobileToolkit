using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A view must implement IView interface
/// </summary>
public class SampleViewManager : MonoBehaviour, IView
{
    [Header("Fields")]
    public LoadingScript LoadingManager;

    /* IView implementations */
    public string GetTitle() => "Sample View";
    public void InitView()
    {
        // Implement your view initialization code
        //InterfaceInteraction.Instance.ShowBackButton();
        InterfaceInteraction.Instance.ShowSidemenuButton();
    }

    public void NavigateBack()
    {
        // Implement your back functionality
        InterfaceInteraction.Instance.NavigatePreviousView();
    }

    /* View methods */

    public void TestLoading()
    {
        StartCoroutine(TestLoadingCoroutine());
    }

    public IEnumerator TestLoadingCoroutine()
    {
        LoadingManager.BeginLoading();

        yield return new WaitForSeconds(3);

        LoadingManager.EndLoading();
    }

}

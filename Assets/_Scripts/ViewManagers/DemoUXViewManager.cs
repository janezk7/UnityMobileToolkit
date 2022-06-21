using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A view must implement IView interface
/// </summary>
public class DemoUXViewManager : ViewManager
{
    [Header("Fields")]
    public GameObject ShrinkButton;
    public LoadingScript GlobalLoadingManager;
    public LoadingScript CustomLoadingManager;


    /* View methods */

    public void OnShrinkButtonPress()
    {
        // shrink
        LeanTween.scale(ShrinkButton, Vector3.zero, 0.5f).setEaseOutExpo();

        // return to normal
        LeanTween.delayedCall(0.8f, () =>
        {
            ShrinkButton.transform.localScale = Vector3.one;
        });
    }

    public void TestGlobalLoading()
    {
        StartCoroutine(TestLoadingCoroutine(GlobalLoadingManager));
    }

    public void TestCustomLoading()
    {
        StartCoroutine(TestLoadingCoroutine(CustomLoadingManager));
    }

    public IEnumerator TestLoadingCoroutine(LoadingScript loadingScript)
    {
        loadingScript.BeginLoading();

        yield return new WaitForSeconds(1);

        loadingScript.EndLoading();
    }
}

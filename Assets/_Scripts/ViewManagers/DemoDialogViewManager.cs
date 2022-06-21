using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDialogViewManager : ViewManager
{
    public ErrorManager ErrorManager;
    public ErrorManager CustomErrorManager;
    public TMPro.TextMeshProUGUI ChangingText;

    public void ShowAlert()
    {
        StartCoroutine(ErrorManager.ShowError("This is a message"));
    }

    public void ShowAlertConnection()
    {
        StartCoroutine(ErrorManager.ShowNoConnectionError());
    }

    public void ShowAlertCallback()
    {
        StartCoroutine(ErrorManager.ShowError("This is a message with custom callback", "Update text and close", () =>
        {
            ChangingText.text = $"App opened for {(int)Time.realtimeSinceStartup} seconds.";
        }));
    }

    public void ShowCustomErrorManager()
    {
        StartCoroutine(CustomAlertCoroutine());
    }


    private IEnumerator CustomAlertCoroutine()
    {
        yield return StartCoroutine(CustomErrorManager.ShowError("This is alert!"));

        yield return new WaitForSeconds(3.0f);

        CustomErrorManager.DismissError();
    }
}

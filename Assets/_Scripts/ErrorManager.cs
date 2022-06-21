using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorManager : MonoBehaviour
{
    public Canvas Canvas;
    public TMPro.TextMeshProUGUI ErrorText;

    [Header("Button")]
    public UnityEngine.UI.Button Button;
    public TMPro.TextMeshProUGUI ButtonText;

    [Header("NoConnection")]
    public UnityEngine.UI.Image NoConnectionImage;

    private bool IsErrorPromptActive = false;

    public void Awake()
    {
        ResetErrorState();
    }

    public IEnumerator ShowNoConnectionError()
    {
        NoConnectionImage.enabled = true;


        yield return ShowError("No connection. Try reloading", "Reload", () => 
        {
            ReInitApp();
        }, ShowNoConnectionImage: true);
    }

    public IEnumerator ShowError(string errorMsg, bool reloadApp = false)
    {
        string buttonText = "Close";
        if(reloadApp)
        {
            buttonText = "Close";
        }
        yield return ShowError(errorMsg, buttonText, () => {
            if (reloadApp)
                ReInitApp();
        });
    }

    public IEnumerator ShowError(string errorMsg, string buttonText, Action buttonCallback, bool ShowNoConnectionImage = false)
    {
        ResetErrorState();
        IsErrorPromptActive = true;
        ErrorText.text = errorMsg;
        ButtonText.text = buttonText;
        Button.onClick.AddListener(() =>
        {
            ResetErrorState();
            buttonCallback();
            IsErrorPromptActive = false;
        });

        NoConnectionImage.enabled = ShowNoConnectionImage;
        Canvas.enabled = true;
        yield return null;
        /*while(IsErrorPromptActive)
        {
            yield return null;
        }*/
    }

    public void DismissError()
    {
        ResetErrorState();
    }

    private void ReInitApp()
    {
        GlobalControl.Instance.ReInitApp();
    }

    private void ResetErrorState()
    {
        Canvas.enabled = false;
        ErrorText.text = "";
        ButtonText.text = "";
        NoConnectionImage.enabled = false;
        Button.onClick.RemoveAllListeners();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public Sidemenu Sidemenu;
    private Canvas Canvas;

    public bool IsOpen { get { return Canvas.enabled; } }

    void Start()
    {
        Canvas = gameObject.GetComponent<Canvas>();
        Canvas.enabled = false;
    }

    public void ToggleOverlay(bool show)
    {
        if (show)
            ShowOverlay();
        else
            HideOverlay();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //    ShowOverlay();
        //else if (Input.GetKeyDown(KeyCode.Y))
        //    HideOverlay();
    }

    public void ShowOverlay()
    {
        Canvas.enabled = true;
        Sidemenu.FadeIn();
    }

    public void HideOverlay()
    {
        Canvas.enabled = false;
        // TODO: Some nice closing effect... maybe slide the whole thing down?
        Sidemenu.FadeOut();
    }
}

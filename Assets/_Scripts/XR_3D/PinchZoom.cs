using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Camera cam;
    public TMPro.TextMeshProUGUI ZoomFactorText;
    float MouseZoomSpeed = 15.0f;
    float TouchZoomSpeed = 0.05f;
    float ZoomMinBound = 10.0f; // 0.1
    float ZoomMaxBound = 170.0f; // 179.9f

    float originalFieldOfView;
    float? modelResizeFactor = null;

    void Start()
    {
        originalFieldOfView = cam.fieldOfView;
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            // Pinch to zoom
            if (Input.touchCount == 2)
            {

                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Zoom(deltaDistance, TouchZoomSpeed);
            }
        }
        else
        {

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Zoom(-scroll, MouseZoomSpeed);
        }



        if (cam.fieldOfView < ZoomMinBound)
        {
            cam.fieldOfView = ZoomMinBound;
        }
        else
        if (cam.fieldOfView > ZoomMaxBound)
        {
            cam.fieldOfView = ZoomMaxBound;
        }
    }

    /// <summary>
    /// Set factor that was used for model resize. Used for UI update.
    /// </summary>
    public void SetResizeFactor(float resizeFactor)
    {
        modelResizeFactor = resizeFactor;
        UpdateZoomText();
    }

    public void ResetZoom()
    {
        //cam.fieldOfView = originalFieldOfView;
        LeanTween.value(cam.gameObject, ResettingCallback, cam.fieldOfView, originalFieldOfView, 0.3f).setEaseOutExpo();
        UpdateZoomText();
    }

    void ResettingCallback(float newFOV)
    {
        cam.fieldOfView = newFOV;
        UpdateZoomText();
    }

    void UpdateZoomText()
    {
        if (ZoomFactorText == null)
            return;
        var fovZoomFactor = originalFieldOfView / cam.fieldOfView;
        var normalizedZoomFactor = fovZoomFactor;
        if(modelResizeFactor.HasValue)
            normalizedZoomFactor = modelResizeFactor.Value * fovZoomFactor;
        ZoomFactorText.text = string.Format("x{0}", normalizedZoomFactor.ToString("F2"));
    }

    void Zoom(float deltaMagnitudeDiff, float speed)
    {

        cam.fieldOfView += deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, ZoomMinBound, ZoomMaxBound);
        UpdateZoomText();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelRotateManager : MonoBehaviour
{
    public Camera Camera;
    public bool RotateVertically = true;

    private Touch touch;
    private float rotateSpeedModifier = 0.1f;

    private float yEuler;
    private float xEuler;

#if UNITY_EDITOR
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;
#endif

    private bool isRotating = false;

    public void ResetRotation()
    {
        TweenUtil.TweenResetRotation(transform.gameObject);
    }

    private void Update()
    {
        var myCamera = Camera;
#if UNITY_EDITOR
        if (Input.touchCount == 0)
        {
            if(Input.GetMouseButtonDown(0) && !isRotating)
            {
                if (EventSystem.current.IsPointerOverGameObject(-1))
                    return;
                isRotating = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isRotating = false;
            }
            
            if(isRotating && Input.GetMouseButton(0))
            {
                mPosDelta = Input.mousePosition - mPrevPos;
                var rotY = -Vector3.Dot(mPosDelta, myCamera.transform.right);
                transform.Rotate(transform.up, rotY * rotateSpeedModifier, Space.World);
                if(RotateVertically)
                    transform.Rotate(myCamera.transform.right, Vector3.Dot(mPosDelta, myCamera.transform.up) * rotateSpeedModifier, Space.World); // Rotate against X axis
            }

            mPrevPos = Input.mousePosition;
            return;
        }
#endif

        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;
                isRotating = true;
            }

            if(isRotating && touch.phase == TouchPhase.Moved)
            {
                // Proper world space rotation solution
                yEuler = -Vector3.Dot(touch.deltaPosition, myCamera.transform.right) * rotateSpeedModifier;
                transform.Rotate(transform.up, yEuler, Space.World);

                // Rotate along X axis
                if (RotateVertically)
                {
                    xEuler = Vector3.Dot(touch.deltaPosition, myCamera.transform.up) * rotateSpeedModifier;
                    transform.Rotate(myCamera.transform.right, xEuler, Space.World);
                }

                /*
                var yRotValue = -touch.deltaPosition.x * rotateSpeedModifier;
                var xRotValue = touch.deltaPosition.y * rotateSpeedModifier;
                // Old solution
                //Quaternion rotationY = Quaternion.Euler(0f, yRotValue, 0f);
                //Quaternion rotationX = Quaternion.Euler(touch.deltaPosition.y * rotateSpeedModifier, 0f, 0f);
                //transform.rotation *= rotationY;
                //transform.rotation *= rotationX;
                // Nicer solution
                transform.Rotate(Vector3.up, yRotValue, Space.World);
                transform.Rotate(Vector3.right, xRotValue, Space.World);
                */
            }
        }
        else
        {
            isRotating = false;
        }
    }
}

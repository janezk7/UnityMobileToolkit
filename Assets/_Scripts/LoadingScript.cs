using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScript : MonoBehaviour
{
    public GameObject LoadingObject;

    private bool isLoading = false;

    public bool IsLoading => isLoading;

    public void Awake()
    {
        LoadingObject.SetActive(false);
    }

    public void BeginLoading()
    {
        if (isLoading)
            return;
        LoadingObject.SetActive(true);
        isLoading = true;
        LeanTween.rotateAroundLocal(LoadingObject, Vector3.forward, -360f, 0.8f).setLoopClamp();
    }

    public void EndLoading()
    {
        LoadingObject.SetActive(false);
        isLoading = false;
        LeanTween.cancel(LoadingObject);
    }

    private void OnDisable()
    {
        EndLoading();
    }

}

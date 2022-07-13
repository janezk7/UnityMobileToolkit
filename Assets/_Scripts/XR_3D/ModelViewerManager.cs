using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModelViewerManager : MonoBehaviour
{
    [Header("Managers")]
    public ModelLoaderManager ModelLoaderManager;
    public LoadingScript LoadingManager;
    public ErrorManager ErrorManager;

    [Header("Settings")]
    public bool Resize3dModelToFit = true;

    [Header("3d Model viewer")]
    public Transform ModelContainer;
    public Renderer BoundingBox3d;

    [Header("Model manipulation")]
    public PinchZoom PinchZoom;
    public ModelRotateManager ModelRotateManager;

    private float DesiredModelViewerSize { get { return BoundingBox3d.bounds.size.x; } }
    private Coroutine modelRotationCoroutine;

    private bool _isInitialized = false;
    public bool IsInitialized 
    { 
        private set { _isInitialized = value; } 
        get { return _isInitialized; } 
    }

    private IEnumerator Start()
    {
        // Set global error manager
        GlobalControl.Instance.ErrorManager = ErrorManager;

        yield return StartCoroutine(LoadData());
    }

    /// <summary>
    /// Load model data
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadData()
    {
        LoadingManager.BeginLoading();

        var cd = new CoroutineWithData(this, ModelLoaderManager.LoadModel(ModelLoaderManager.FallbackGltfUri));
        yield return cd.coroutine;

        // Cache single model
        var gltfScene = (GameObject)cd.result;
        var model = Instantiate(gltfScene, ModelContainer);

        float maxSize = GameObjectUtil.GetMaxSize(model);
        SetGltfModel(model, maxSize);

        LoadingManager.EndLoading();
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainInterface");
    }

    public void ResetTransformation()
    {
        PinchZoom.ResetZoom();
        ModelRotateManager.ResetRotation();
    }

    /// <summary>
    /// Instantiate and show gltf model
    /// </summary>
    private void SetGltfModel(GameObject gltfModel, float maxSize)
    {
        foreach (Transform child in ModelContainer)
            Destroy(child.gameObject);

        // Instantiate and resize model
        var newGltfModel = Instantiate(gltfModel, ModelContainer);
        var resizeFactor = 1.0f;
        if (Resize3dModelToFit)
            resizeFactor = GameObjectUtil.GetResizeFactor(maxSize, DesiredModelViewerSize);

        newGltfModel.transform.localScale *= resizeFactor;
        PinchZoom.SetResizeFactor(resizeFactor);

        newGltfModel.SetActive(true);
    }

    private void OnDisable()
    {
        StopRotatingModel();
    }

    private void StartRotatingModel()
    {
        modelRotationCoroutine = StartCoroutine(RotateModel());
    }

    private void StopRotatingModel()
    {
        if(modelRotationCoroutine != null)
            StopCoroutine(modelRotationCoroutine);
    }

    private IEnumerator RotateModel()
    {
        while (true)
        {
            var rotDegrees = Time.deltaTime * 10;
            ModelContainer.transform.Rotate(Vector3.up, rotDegrees);
            yield return null;
        }
    }
}

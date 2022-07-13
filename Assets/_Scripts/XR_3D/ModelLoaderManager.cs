using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ModelLoaderManager : MonoBehaviour
{
    [Header("Model loader")]
    public GLTFast.GltfAsset GLTFast;

    [Header("Readonly")]
    public GameObject gltfScene = null;

    [Header("Debug fields")]
    public bool UseFallbackUri = true;
    public string FallbackGltfUri;

    // Main LoadModel method
    public IEnumerator LoadModel(string gltfFilename)
    {
        yield return null;
        var cd_model = new CoroutineWithData(this, LoadModel_glTFast(gltfFilename));
        yield return cd_model.coroutine;
        gltfScene = (GameObject)cd_model.result;

        // Return gltf model!
        yield return gltfScene;
    }

    /// <summary>
    /// glTFast loader (atteneder)
    /// </summary>
    private IEnumerator LoadModel_glTFast(string gltfFilename)
    {
        var loader = GLTFast;
        if (!UseFallbackUri)
        {
            if(gltfFilename is null)
            {
                Debug.LogWarning("No gltfFilename passed. Aborting");
                yield return null;
                yield break;
            }
            loader.url = gltfFilename;
        }
        else
            loader.url = FallbackGltfUri;

        Debug.Log("Loading model (glTFast): " + loader.url);
        // First step: load glTF
        var logger = new GLTFast.Logging.ConsoleLogger();
        var gltfImport = new GLTFast.GltfImport(logger: logger);

        var gltfTask = gltfImport.Load(loader.url);
        yield return new WaitUntil(() => gltfTask.IsCompleted);

        var success = gltfTask.Result;
        if (success)
        {
            var loadedSceneParent = new GameObject("loadedScene");
            loadedSceneParent.transform.parent = GLTFast.gameObject.transform;

            gltfImport.InstantiateMainScene(loadedSceneParent.transform);

            yield return loadedSceneParent;
            

            //if(loadedSceneParent.transform.childCount == 0)
            //{
            //    Debug.LogError("glTF object has no scene!");
            //    yield return null;
            //    yield break;
            //}

            //var loadedScene = loadedSceneParent.transform.GetChild(0);
            //loadedScene.gameObject.SetActive(false);

            //// Get single child scene parent object (correct pivoting)
            //if(loadedScene.childCount == 0)
            //{
            //    Debug.LogError("glTF object has empty scene!");
            //    yield return null;
            //    yield break;
            //}

            //if(loadedScene.childCount > 1)
            //    Debug.LogError("Expected a single parent child! Pivot might not be correct.");

            //var sceneParentObject = loadedScene.GetChild(0).gameObject;
            //sceneParentObject.name = "sceneParentObject";

            //yield return sceneParentObject;
        }
        else
        {
            Debug.LogError("Loading glTF failed!");
            yield return null;
        }
    }
}

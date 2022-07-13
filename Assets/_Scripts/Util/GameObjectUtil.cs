using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectUtil
{
    public static float GetMaxSize(GameObject parentObject)
    {
        var renderers = parentObject.GetComponentsInChildren<Renderer>();

        // Get center
        var center = Vector3.zero;
        foreach (var rend in renderers)
            center += rend.bounds.center;
        center /= renderers.Length;

        // Get bounds
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (var rend in renderers)
            bounds.Encapsulate(rend.bounds);

        var gltfSize = bounds.size;
        var maxSize = Mathf.Max(gltfSize.x, Mathf.Max(gltfSize.y, gltfSize.z));
        return maxSize;
    }

    public static float GetResizeFactor(float maxSize, float desiredSize)
    {
        return desiredSize / maxSize;
    }
}

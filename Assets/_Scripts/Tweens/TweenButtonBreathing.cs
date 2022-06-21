using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TweenButtonBreathing : MonoBehaviour
{
    Vector3 InitialScale;
    // Start is called before the first frame update
    void Start()
    {
        InitialScale = gameObject.transform.localScale;
        var scaledSize = InitialScale * 1.05f;
        LeanTween.scale(gameObject, scaledSize, 0.7f).setLoopPingPong();
    }
}

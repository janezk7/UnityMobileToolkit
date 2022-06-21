using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageManager : MonoBehaviour
{
    public UnityEngine.UI.Image Image;
    public int SpriteHeight;

    void Start()
    {
        Image.sprite = null;   
    }

    public void SetImage(Sprite sprite)
    {
        Image.sprite = sprite;
        Image.enabled = true;
    }

}

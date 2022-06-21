using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryCardManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Title;

    [Header("Image")]
    public int SpriteHeight;
    public Image Img;
    public Image PlaceholderImg;

    public void Init(Category category)
    {
        Title.text = category.Name;
        // Hardcoded icons method
        /*
        var sprite = Resources.Load<Sprite>("Icons/"+ category.SeoFilename);
        //var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if(sprite is null)
        {
            Icon.enabled = false;
            return;
        }
        Icon.sprite = sprite;
        */
        if (category.PictureUrlHttp is null)
        {
            Img.enabled = false;
            return;
        }
        StartCoroutine(LoadImage(category.PictureUrlHttp));
    }

    private IEnumerator LoadImage(string imageUrl)
    {
        var cd = new CoroutineWithData(this, GlobalControl.Instance.LoadAndCacheImage(imageUrl, SpriteHeight));
        yield return cd.coroutine;
        Sprite sprite = cd.result as Sprite;
        PlaceholderImg.enabled = false;
        if (sprite is null)
            yield break;

        Img.sprite = sprite;
    }
}

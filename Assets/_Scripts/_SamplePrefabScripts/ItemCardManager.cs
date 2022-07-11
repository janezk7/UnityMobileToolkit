using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Assets.Scripts.Util;

/// <summary>
/// Sample item card manager
/// </summary>
public class ItemCardManager : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Subtitle;
    public TextMeshProUGUI Price;
    public UnityEngine.UI.Button Button;

    [Header("Image")]
    public int SpriteHeight;
    public UnityEngine.UI.Image Img;
    public UnityEngine.UI.Image PlaceholderImg;

    public void Init(string title, string subtitle, string imageUrl, UnityAction OnClickCallback)
    {
        Title.text = title;
        Subtitle.text = subtitle;
        Button.onClick.AddListener(OnClickCallback);

        StartCoroutine(LoadImage(imageUrl));
    }

    private IEnumerator LoadImage(string imageUrl)
    {
        var cd = new CoroutineWithData(this, GlobalControl.Instance.LoadAndCacheImage(imageUrl, SpriteHeight));
        yield return cd.coroutine;
        Sprite sprite = cd.result as Sprite;
        PlaceholderImg.enabled = false;
        if(sprite is null)
            yield break;

        Img.sprite = sprite;
    }
}

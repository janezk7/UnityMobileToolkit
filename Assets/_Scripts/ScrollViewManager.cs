using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// Manager responsible for images and pagination
/// </summary>
public class ScrollViewManager : MonoBehaviour
{
    public RectTransform ContentRectTransform;
    public Scrollbar ScrollBarHorizontal;
    public Scrollbar ScrollBarVertical;

    [Header("ScrollSnap")]
    [SerializeField]
    private PaginationManager PaginationManager;
    [SerializeField]
    private HorizontalScrollSnap HorizontalScrollSnap;

    [Header("Prefabs")]
    public GameObject ImageCard;
    public GameObject Toggle;

    public List<ImageManager> ImageManagers { get; private set; }

    public void GoToPage(int pageNr)
    {
        PaginationManager.GoToScreen(pageNr);
    }

    /// <summary>
    /// Initialized pagination and image placeholders
    /// </summary>
    /// <param name="imageCount"></param>
    public IEnumerator Init(int imageCount)
    {
        HorizontalScrollSnap.enabled = false;
        HorizontalScrollSnap.CurrentPage = 0; // Triggers screen change and page referencing
        yield return null;

        Cleanup();

        // Instantiate images and paginations
        ImageManagers = new List<ImageManager>();
        ImageManagers.Clear();

        for (int i = 0; i < imageCount; i++)
        {
            var imageObject = Instantiate(ImageCard, ContentRectTransform.transform);
            var manager = imageObject.GetComponent<ImageManager>();
            ImageManagers.Add(manager);
            AddPageToPagination();
        }

        // One must exist
        if (imageCount == 0)
            AddPageToPagination();

        PaginationManager.ResetPaginationChildren();
        HorizontalScrollSnap.UpdateLayout();
        yield return null;
        HorizontalScrollSnap.enabled = true;
    }

    public void Cleanup()
    {
        // Clear toggle pagination
        var m_PaginationChildren = PaginationManager.gameObject.GetComponentsInChildren<Toggle>().ToList();
        foreach (var toggle in m_PaginationChildren)
            DestroyImmediate(toggle.gameObject);

        // Clear images
        var children = HorizontalScrollSnap.ChildObjects; // Gotta use ScrolSnap children. Getting all child transforms from container doesn't work for some reason
        foreach (var cardObject in children)
            DestroyImmediate(cardObject);

        // Reset content rect position
        ContentRectTransform.offsetMin = new Vector2(0.0f, ContentRectTransform.offsetMin.y);
        ContentRectTransform.offsetMax = new Vector2(0.0f, ContentRectTransform.offsetMax.y);
    }

    public void ResetAllScrolls()
    {
        ResetVerticalScroll();
        ResetHorizontalScroll();
    }

    public void ResetVerticalScroll()
    {
        if (ContentRectTransform == null)
        {
            Debug.LogError("Missing key components! Please set in inspector");
            return;
        }

        var contentPos = ContentRectTransform.anchoredPosition;
        ContentRectTransform.anchoredPosition = new Vector2(contentPos.x, 0.0f);

        if(ScrollBarVertical != null)
            ScrollBarVertical.value = 0;
    }

    public void ResetHorizontalScroll()
    {
        if (ContentRectTransform == null)
        {
            Debug.LogError("Missing key components! Please set in inspector");
            return;
        }

        var contentPos = ContentRectTransform.anchoredPosition;
        
        LeanTween.value(ContentRectTransform.gameObject, (float val) => {
            ContentRectTransform.anchoredPosition = new Vector2(val, contentPos.y);
        }, contentPos.x, 0.0f, 0.3f).setEaseOutExpo();

        if(ScrollBarHorizontal != null)
            ScrollBarHorizontal.value = 0;
    }

    private void AddPageToPagination()
    {
        Instantiate(Toggle, PaginationManager.transform);
        //PaginationManager.ResetPaginationChildren();
        //HorizontalScrollSnap.UpdateLayout();
    }
}

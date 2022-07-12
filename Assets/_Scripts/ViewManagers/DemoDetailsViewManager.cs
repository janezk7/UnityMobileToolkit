using Assets._Scripts.Entities;
using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DemoDetailsViewManager : ViewManager
{
    [Header("Managers")]
    public ScrollViewManager ImagesScrollManager;
    public LoadingScript LoadingManager;

    [Header("Details elements")]
    public GameObject InterfaceContent;
    public TextMeshProUGUI AssetTitle;
    public TextMeshProUGUI Description;
    public Transform SpecificationListContainer;

    [Header("Prefabs")]
    public GameObject ImageCard;
    //public GameObject SpecificationCard;
    public GameObject ListSeperator;

    public override void InitView()
    {
        base.InitView();

        StartCoroutine(LoadProduct());
    }

    private IEnumerator LoadProduct()
    {
        // Clear interface
        InterfaceContent.SetActive(false);
        AssetTitle.text = "...";
        Description.text = "...";

        LoadingManager.BeginLoading();

        // Load product 
        var cd = new CoroutineWithData(this, DependancyProvider.Services.AssetService.GetAssetDetails(this, 1));
        yield return cd.coroutine;
        var apiResponse = cd.result as ApiResponse;
        if(!apiResponse.Ok)
        {
            yield return GlobalControl.Instance.ErrorManager.ShowError("Error fetching product: " + apiResponse.ErrorMessage);
            yield break;
        }

        DependancyProvider.Services.AssetService.AssetCache = (Asset)apiResponse.Data;
        InterfaceContent.SetActive(true);

        UpdateUIFields();

        LoadingManager.EndLoading();
    }

    private void UpdateUIFields()
    {
        var product = DependancyProvider.Services.AssetService.AssetCache;

        AssetTitle.text = product.Name;
        Description.text = product.Description;

        StartCoroutine(PopulateImages());
        //StartCoroutine(UpdateSpecifications());
    }

    IEnumerator PopulateImages()
    {
        var product = DependancyProvider.Services.AssetService.AssetCache;

        yield return null;

        // Init images and pagination
        int productPictureCount = product.PictureList.Length;
        var imageCount = productPictureCount;
        yield return StartCoroutine(ImagesScrollManager.Init(imageCount));


        // Load and set all product images
        //var operations = new List<Coroutine>();
        for (int i = 0; i < productPictureCount; i++)
        {
            var img = product.PictureList[i];
            yield return StartCoroutine(LoadAndUpdateContainerImage(imageIndex: i, img.PictureUrl));
        }
    }

    IEnumerator LoadAndUpdateContainerImage(int imageIndex, string pictureUrl, bool cacheImage = false)
    {
        var imageManager = ImagesScrollManager.ImageManagers[imageIndex];

        var height = imageManager.SpriteHeight;
        var cd = new CoroutineWithData(this, GlobalControl.Instance.LoadAndCacheImage(pictureUrl, height, cacheImage: cacheImage));
        // TODO Optimize async loading
        yield return cd.coroutine;
        yield return imageManager;

        // Update image
        var sprite = cd.result as Sprite;
        if (sprite is null)
        {
            Debug.Log("Error loading image: " + pictureUrl);
            yield break;
        }

        imageManager.SetImage(sprite);
    }

    /*
    IEnumerator UpdateSpecifications()
    {
        var product = DependancyProvider.Services.AssetService.AssetCache;

        // Clear specifications
        foreach (Transform child in SpecificationListContainer)
            Destroy(child.gameObject);

        yield return null;

        // Group specifications
        Dictionary<string, List<string>> groupedSpecifications = new Dictionary<string, List<string>>();
        foreach (var s in product.SpecificationList)
        {
            if (groupedSpecifications.ContainsKey(s.Specification))
            {
                groupedSpecifications[s.Specification].Add(s.Value);
                continue;
            }

            groupedSpecifications.Add(s.Specification, new List<string>() { s.Value });
        }

        foreach (var s in groupedSpecifications)
        {
            var card = Instantiate(SpecificationCard, SpecificationListContainer);
            var seperator = Instantiate(ListSeperator, SpecificationListContainer);
            var manager = card.GetComponent<SpecificationCardManager>();
            manager.SetKeyValue(s.Key, string.Join(", ", s.Value));

            yield return null;
        }
    }
    */

}

using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sample view for loading and populating items
/// </summary>
public class ItemGalleryManager : ViewManager
{
    [Header("Settings")]
    public bool AddSeperator = true;

    [Header("Fields")]
    public Transform ItemList;
    public LoadingScript LoadingManager;
    public GameObject NoItemsContent;

    [Header("Prefabs")]
    public GameObject ItemCard;
    public GameObject ListSeperator;

    private Action CardClickCallback = () => { };

    public override void InitView()
    {
        base.InitView();
        ClearProductCards();

        // Load on start?
        //var filterObject = new FilterObject(null, null);
        //StartCoroutine(LoadAndPopulate(filterObject));
    }

    public void SetCardClickCallback(Action callback)
    {
        CardClickCallback = callback;
    }

    public void OnLoadDataButtonClick()
    {
        StartCoroutine(LoadAndPopulate(new FilterObject(null, null)));
    }

    public void OnClearDataButtonClick()
    {
        ClearProductCards();
    }

    public IEnumerator LoadAndPopulate(FilterObject filterObject)
    {
        LoadingManager.BeginLoading();

        // Simulate loading
        yield return new WaitForSeconds(1.0f);

        // Clear content area
        ClearProductCards();
        NoItemsContent.SetActive(false);

        // Load and cache item data in GlobalControl
        yield return StartCoroutine(GlobalControl.Instance.LoadAndCacheItems(filterObject.ApiQueryObject));

        // Populate cached items in GlobalControl
        yield return StartCoroutine(PopulateItemList(filterObject.ManualFilterObject));
        LoadingManager.EndLoading();
    }

    private void ClearProductCards()
    {
        foreach (Transform child in ItemList)
            Destroy(child.gameObject);
        NoItemsContent.SetActive(true);
    }

    private IEnumerator PopulateItemList(ManualFilterObject filterObject)
    {
        var items = GlobalControl.Instance.ItemsCache.Items;

        if(items is null)
        {
            yield break;
        }

        if (items.Count == 0)
        {
            NoItemsContent.SetActive(true);
            yield break;
        }

        // Manually filter products
        if (filterObject.HasFilter)
        {
            Debug.Log("Has filter. Filtering!");
            items = filterObject.GetFilteredItems(items);
        }

        foreach (var itemData in items)
        {
            // Register onClick callback when user touches item
            var onClickCallback = new UnityEngine.Events.UnityAction(() =>
            {
                CardClickCallback();
                // TODO: Implement item click callback
                Debug.Log("Clicked on item: "+ itemData.ToString());
            });

            var assetCard = Instantiate(ItemCard, ItemList);
            string sampleIconUrl = "https://cdn-icons-png.flaticon.com/512/616/616554.png";
            assetCard.GetComponent<ItemCardManager>().Init(itemData.Name, itemData.Description, sampleIconUrl, onClickCallback);

            if(AddSeperator)
                Instantiate(ListSeperator, ItemList);
            yield return null;
        }
    }
}

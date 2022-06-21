using Assets.Scripts.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager for search overlay (search, filtering, sorting)
/// </summary>
public class SearchViewManager : MonoBehaviour
{
    [Header("Manager Scripts")]
    [SerializeField]
    private SearchBarManager SearchBarManager;
    [SerializeField]
    private FilterManager FilterManager;
    [SerializeField]
    private OverlayManager OverlayManager;
    private ItemGalleryManager SearchGalleryManager;

    // Filter fields
    [HideInInspector]
    public string SearchString;

    private Coroutine ActiveSearchQueryCoroutine = null;

    public bool IsOverlayShown { get; private set; } = false;
    public bool IsFilterOverlayShown { get; set; } = false;

    private void Start()
    {
        SearchGalleryManager = GetComponent<ItemGalleryManager>();
        if (SearchGalleryManager is null)
        {
            Debug.LogError("GalleryManager script is required on the same GameObject!");
            return;
        }

        // Hide overlay on card click
        SearchGalleryManager.SetCardClickCallback(ToggleSearchOverlay);
    }

    public void ToggleSearchOverlay()
    {
        var shouldOpenSearchOverlay = !IsOverlayShown;

        // TODO Implement when filter overlay should show instead
        bool showFilterOverlay = false; 
        if (showFilterOverlay && shouldOpenSearchOverlay)
        {
            ShowSearchOverlayWithFilter();
            return;
        }

        IsOverlayShown = shouldOpenSearchOverlay;
        InitializeSearchOverlay();
    }

    public void ShowSearchOverlayWithFilter()
    {
        // Open filter panel
        FilterManager.ToggleOverlay(true);

        var openSearchOverlay = !IsOverlayShown;
        if(openSearchOverlay)
        {
            IsOverlayShown = true;
            InitializeSearchOverlay();
        }
    }

    public void HandleBackNavigation()
    {
        // First close filter overlay if active
        if(IsFilterOverlayShown)
        {
            StartCoroutine(FilterManager.CloseOverlay());
            return;
        }

        if(!IsOverlayShown)
        {
            Debug.LogError("Invalid search overlay back handling. Overlay not active!");
            return;
        }

        // Close search overlay
        ToggleSearchOverlay();
    }

    public void TriggerSearch()
    {
        Debug.Log("Triggering search: "+ SearchString);
        var filterObject = new FilterObject(
            SearchString,
            FilterManager.SelectedSort
        );

        if (ActiveSearchQueryCoroutine != null)
        {
            StopCoroutine(ActiveSearchQueryCoroutine);
            SearchGalleryManager.StopAllCoroutines();
            Debug.Log("Active search coroutine stopped.");
        }
        ActiveSearchQueryCoroutine = StartCoroutine(LoadAndPopulateProducts(filterObject));
    }

    private void InitializeSearchOverlay(bool hideFilterOverlay = false)
    {
        if(hideFilterOverlay)
        {
            FilterManager.ToggleOverlay(isOpening:false);
        }

        OverlayManager.ToggleOverlay(IsOverlayShown);

        SearchBarManager.InitUI();
        FilterManager.InitFields();
    }

    private IEnumerator LoadAndPopulateProducts(FilterObject filterObject)
    {
        yield return StartCoroutine(SearchGalleryManager.LoadAndPopulate(filterObject));

        ActiveSearchQueryCoroutine = null;
    }
}

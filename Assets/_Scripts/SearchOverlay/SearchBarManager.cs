using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manager for top search bar input and controls
/// </summary>
public class SearchBarManager : MonoBehaviour
{
    [Header("Manager Scripts")]
    public EventSystem EventSystemManager;

    [Header("Search Elements")]
    public Sprite SearchIcon;
    public Sprite SearchCancelIcon;
    public UnityEngine.UI.Image ButtonImage;
    public TMPro.TMP_InputField InputField;

    [Header("Search Overlay")]
    public SearchViewManager SearchViewManager;

    private void Start()
    {
        UpdateSearchElements();
    }

    public void ToggleSearchBar(bool show)
    {
        gameObject.SetActive(show);
    }

    public void InitUI()
    {
        var isSearch = SearchViewManager.IsOverlayShown;

        UpdateSearchElements();
        if(isSearch)
            ClearInputFieldAndSearch();
    }

    public void OnTextChanged(string searchString)
    {
        StopAllCoroutines();
        SearchViewManager.SearchString = searchString;
        var queryString = SearchViewManager.SearchString;
        var skipSearch = queryString.Length < 3 && queryString.Length != 0;
        if (skipSearch)
        {
            return;
        }
        StartCoroutine(QueueSearchTrigger(queryString));
    }

    private IEnumerator QueueSearchTrigger(string queryString)
    {
        if(!string.IsNullOrEmpty(queryString))
            yield return new WaitForSeconds(0.5f);

        SearchViewManager.TriggerSearch();
        yield return null;
    }

    public void ClearInputFieldAndSearch()
    {
        SearchViewManager.SearchString = string.Empty;
        InputField.text = SearchViewManager.SearchString;
        SearchViewManager.TriggerSearch();
        var isFilterOverlayInactive = !SearchViewManager.IsFilterOverlayShown;
        if(isFilterOverlayInactive)
            FocusOnInputField();
    }

    private void UpdateSearchElements()
    {
        var isSearch = SearchViewManager.IsOverlayShown;
        InputField.gameObject.SetActive(isSearch);
        ButtonImage.sprite = isSearch ? SearchCancelIcon : SearchIcon;

        // Don't focus on input field if filter overlay is active (keyboard covers filters otherwise!)
        var isFilterOverlayInactive = !SearchViewManager.IsFilterOverlayShown;
        if(isSearch && isFilterOverlayInactive)
            FocusOnInputField();
    }

    private void FocusOnInputField()
    {
        EventSystemManager.SetSelectedGameObject(InputField.gameObject, null);
        var ped = new PointerEventData(EventSystemManager)
        {
            button = PointerEventData.InputButton.Left
        };
        InputField.OnPointerClick(ped);
    }
}

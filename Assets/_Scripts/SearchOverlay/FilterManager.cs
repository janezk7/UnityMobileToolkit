using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class FilterManager : MonoBehaviour
{
    [Header("Managers")]
    public SearchViewManager SearchViewManager;
    private Sidemenu Sidemenu;

    [Header("Filter elements")]
    public TMPro.TMP_Dropdown Dropdown;

    [Space(10)]
    public bool DelaySearchTrigger = true;

    // Internal fields
    public SortType SelectedSort;

    // TODO: Add custom fields 
    public bool IncludeCategory;

    public enum SortType
    {
        Alphabetical = 0,
        AlphabeticalDesc = 1,
        // TODO: Add sort types
    }

    private void Awake()
    {
        Sidemenu = GetComponent<Sidemenu>();
        InitFields();
    }

    public void InitFields()
    {
        StartCoroutine(PopulateSortOptions());
        ResetFields();
        UpdateUI();
    }

    public void ClearCategory()
    {
        IncludeCategory = false;
        UpdateUI();
    }

    public void RevertFields()
    {
        ResetFields();
        UpdateUI();
        //SearchViewManager.TriggerSearch();
        //HideOverlay();
    }

    public void ApplyFilters()
    {
        // TODO Implementation: Apply changes
        

        StartCoroutine(HideOverlayAndSearch());
    }

    public IEnumerator CloseOverlay()
    {
        HideOverlay();
        yield return new WaitForSeconds(Sidemenu.AnimationTime);
        RevertFields();
    }

    public IEnumerator HideOverlayAndSearch()
    {
        HideOverlay();
        yield return null;
        if(DelaySearchTrigger)
            yield return new WaitForSeconds(Sidemenu.AnimationTime);
        SearchViewManager.TriggerSearch();
    }

    public void OnDropdownValueChanged(int value)
    {
        SelectedSort = (SortType)value;
    }

    public void ToggleOverlay(bool isOpening)
    {
        UpdateUI();
        SearchViewManager.IsFilterOverlayShown = isOpening;
        Sidemenu.ToggleSideMenu(isOpening);
    }

    public void ShowOverlay()
    {
        ToggleOverlay(true);
    }

    public void HideOverlay()
    {
        ToggleOverlay(false);
    }

    private IEnumerator PopulateSortOptions()
    {
        Dropdown.ClearOptions();
        var locTable = GlobalControl.Instance.CurrentStringTable?.TableCollectionName;
        if (locTable is null)
            yield break;

        var opOption1 = new LocalizedString(locTable, "Name_asc").GetLocalizedString();
        var opOption2 = new LocalizedString(locTable, "Name_desc").GetLocalizedString();

        Dropdown.AddOptions(new List<string>()
        {
            opOption1,
            opOption2
            // TODO: add options to dropdown
        });
    }

    private void ResetFields()
    {
        SelectedSort = SortType.Alphabetical;
    }

    private void UpdateUI()
    {
        // Update sort
        Dropdown.value = (int)SelectedSort;
        Dropdown.RefreshShownValue();

        // TODO: update other overlay fields
    }
}

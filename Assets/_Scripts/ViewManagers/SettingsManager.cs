using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class SettingsManager : MonoBehaviour, IView
{
    [Header("Fields")]
    public TMPro.TextMeshProUGUI AppVersion;
    public GameObject ClearAppDataText;

    [Header("Languages")]
    public Color LanguageTextColor;
    public Color SelectedLanguageTextColor;
    public TextMeshProUGUI[] LanguageTexts;

    public string GetTitle() => "Settings";

    public void InitView()
    {
        InterfaceInteraction.Instance.ShowBackButton();
        var userSettings = GlobalControl.Instance.UserSettings;
        StartCoroutine(RefreshSettings(false));

        UpdateLanguageUI();

        AppVersion.text = string.Format("Ver {0}", Application.version);
        ClearAppDataText.SetActive(false);
    }

    public void NavigateBack()
    {
        InterfaceInteraction.Instance.NavigatePreviousView();
    }

    public void SetLanguage(int localeIndex)
    {
        GlobalControl.Instance.UserSettings.SetLocale(localeIndex);
        StartCoroutine(RefreshSettings(updateLocalizationTable: true));

        UpdateLanguageUI();
    }

    /// <summary>
    /// Delete all saved data. Favourites and settings
    /// </summary>
    public void ResetAppData()
    {
        UserSettingsUtil.ClearAppData();
        GlobalControl.Instance.InitializeUserSettings();
        InitView();
        ClearAppDataText.SetActive(true);
    }

    private IEnumerator RefreshSettings(bool updateLocalizationTable)
    {
        if(updateLocalizationTable)
            yield return StartCoroutine(InterfaceInteraction.Instance.UpdateCurrentLocalizationTable());
        yield break;
    }

    private void UpdateLanguageUI()
    {
        var localeIndex = GlobalControl.Instance.UserSettings?.LocaleIndex ?? 0;
        for (int i = 0; i < LanguageTexts.Length; i++)
            LanguageTexts[i].color = i == localeIndex ? SelectedLanguageTextColor : LanguageTextColor;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[CreateAssetMenu]
[Serializable]
public class UserSettings : ScriptableObject
{
    public bool IsFirstTime = true;
    public int LocaleIndex;
    public bool IsCacheImages = true;
    public bool ShowVerboseMessages = true;

    public int MaxImageCache { get; private set; } = 30;

    public int LanguageId { get { return LocaleIndex + 1; } }

    public void SetLocale(int index)
    {
        LocaleIndex = index;
        UserSettingsUtil.SaveUserSettings(this);
        ApplyLanguage();
    }

    public void SetImageCache(bool isCacheImages)
    {
        IsCacheImages = isCacheImages;
        UserSettingsUtil.SaveUserSettings(this);
    }

    public Locale GetLocale()
    {
        var localesProvider = LocalizationSettings.Instance.GetAvailableLocales();
        return localesProvider.Locales[LocaleIndex];
    }

    public void SaveSettings()
    {
        UserSettingsUtil.SaveUserSettings(this);
    }

    public void ClearSettings()
    {
        UserSettingsUtil.ClearSettings();
    }

    public void ApplySettings()
    {
        ApplyLanguage();
        // TODO: Add implementation for any added settings
    }

    private void ApplyLanguage()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        /*
        Debug.Log("******** Locale index: " + LocaleIndex);
        Debug.Log("******** locales: " + locales.Count);
        foreach (var locale in locales)
            Debug.Log("+++++ locale: " + locale.name);
        */
        LocalizationSettings.SelectedLocale = locales[LocaleIndex];

        // Clear cache
        GlobalControl.Instance.ClearCache();
    }

    #region Localization methods

    //public void SetLocale(int index)
    //{
    //    LocaleIndex = index;
    //    UserSettingsUtil.SaveUserSettings(this);
    //    ApplyLanguage();
    //}

    //public Locale GetLocale()
    //{
    //    var localesProvider = LocalizationSettings.Instance.GetAvailableLocales();
    //    return localesProvider.Locales[LocaleIndex];
    //}

    //private void ApplyLanguage()
    //{
    //    var locales = LocalizationSettings.AvailableLocales.Locales;
    //    /*
    //    Debug.Log("******** Locale index: " + LocaleIndex);
    //    Debug.Log("******** locales: " + locales.Count);
    //    foreach (var locale in locales)
    //        Debug.Log("+++++ locale: " + locale.name);
    //    */
    //    LocalizationSettings.SelectedLocale = locales[LocaleIndex];

    //    // Clear cache
    //    GlobalControl.Instance.ClearCache();
    //}

    #endregion
}

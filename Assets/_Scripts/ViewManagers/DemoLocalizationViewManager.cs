using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DemoLocalizationViewManager : ViewManager
{
    public TMPro.TextMeshProUGUI SelectedLanguageValue;
    public TMPro.TextMeshProUGUI LocalizedText;

    public override void InitView()
    {
        base.InitView();

        UpdateLocaleText();
        StartCoroutine(UpdateLocalizedString());
    }

    private void UpdateLocaleText()
    {
        SelectedLanguageValue.text = LocalizationSettings.SelectedLocale.LocaleName;
    }

    private IEnumerator UpdateLocalizedString()
    {
        var key = "Welcome_message";
        var cd = new CoroutineWithData(this, GetLocalizedString(key));
        yield return cd;

        LocalizedText.text = (string)cd.result;
    }

    /// <summary>
    /// Get localized string programmatically. Pass table name and key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private IEnumerator GetLocalizedString(string key)
    {
        var stringOp = new LocalizedString(InterfaceInteraction.CurrentStringTable.TableCollectionName, key);
        var locString = stringOp.GetLocalizedString();

        yield return locString;
    }
}

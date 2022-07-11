using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCardManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Label;
    [SerializeField]
    private UnityEngine.UI.Toggle Toggle;

    public bool IsChecked => Toggle.isOn;

    public string LabelText { get { return Label.text; } }

    public void SetLabelText(string text)
    {
        Label.text = text ?? "Unknown";
    }

    public void SetToggleStatus(bool isOn)
    {
        Toggle.isOn = isOn;
    }
}

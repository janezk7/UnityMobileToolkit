using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class TooltipTextFade : MonoBehaviour
{
    TMPro.TextMeshProUGUI Text;
    Color InitialColor;

    private float FadeTimeoutSeconds = 8;

    Color ClearColor 
    { 
        get
        {
            var targetColor = InitialColor;
            targetColor.a = 0.0f;
            return targetColor;
        }
    }


    private void Awake()
    {
        Text = GetComponent<TMPro.TextMeshProUGUI>();
        InitialColor = Text.color;
    }

    private void OnEnable()
    {
        StartCoroutine(ActivateTooltip());
    }

    private IEnumerator ActivateTooltip()
    {
        Text.color = InitialColor;
        yield return new WaitForSeconds(FadeTimeoutSeconds);

        LeanTween.value(Text.gameObject, InitialColor, ClearColor, 0.5f)
            .setOnUpdate((Color fadedColor) => {
                Text.color = fadedColor;
            });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TweenUtil
{
    public static void TweenPressRectSize(RectTransform ButtonRect)
    {
        var buttonDim = new Vector2(ButtonRect.rect.width, ButtonRect.rect.height);
        LeanTween.size(ButtonRect, buttonDim * 1.1f, 0.05f).setLoopPingPong(1);
    }

    public static void TweenPressScale(GameObject Button)
    {
        LeanTween.scale(Button, Vector3.one * 1.1f, 0.05f).setLoopPingPong(1);
    }

    public static void TweenResetRotation(GameObject gameObject)
    {
        LeanTween.rotate(gameObject, Vector3.zero, 0.3f).setEaseOutExpo();
    }
}

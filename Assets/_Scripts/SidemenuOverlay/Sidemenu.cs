using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sidemenu : MonoBehaviour
{
    public static float AnimationTime = 0.4f;

    [Header("Fields")]
    public RectTransform ContentTransform;
    public Image BackgroundFadeImage;

    [Header("Settings")]
    public SlideDirection MenuSlideDirection;
    public float FadeMax = 0.5f;
    public float FadeTime = 0.2f;

    private Canvas Canvas;
    private float screenWidth;
    private float screenHeight;
    private float FadePercent = 0f;

    public enum SlideDirection
    {
        Right = 0,
        Left = 1,
        Down = 3,
        Up = 4
    }

    public bool IsOpen { get { return Canvas.enabled; } }

    private void Awake()
    {
        Canvas = gameObject.GetComponent<Canvas>();
        if(Canvas != null)
        {
            var rectTransform = Canvas.gameObject.GetComponent<RectTransform>();
            screenWidth = rectTransform.rect.width;
            screenHeight = rectTransform.rect.height;
            
            ToggleSideMenu(isOpening: false);
        }

        // Initialize fade
        if(BackgroundFadeImage != null)
        {
            BackgroundFadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void FadeIn()
    {
        if (BackgroundFadeImage == null)
            return;
        BackgroundFadeImage.gameObject.SetActive(true);
        LeanTween.value(gameObject, updateFadeAmountCallback, 0.0f, FadeMax, FadeTime);
    }

    public void FadeOut()
    {
        if (BackgroundFadeImage == null)
            return;
        LeanTween.value(gameObject, updateFadeAmountCallback, FadeMax, 0.0f, FadeTime).setOnComplete(() => {
            BackgroundFadeImage.gameObject.SetActive(false);
        });
    }

    void updateFadeAmountCallback(float newValue)
    {
        FadePercent = newValue;
        BackgroundFadeImage.color = new Color(0, 0, 0, newValue);
    }

    public void ToggleSideMenu()
    {
        var isOpening = !IsOpen;
        ToggleSideMenu(isOpening);
    }

    public void ToggleSideMenu(bool isOpening, bool immediatelly = false)
    {
        if (ContentTransform == null)
        {
            Debug.LogWarning("No content canvas. Aborting toggle");
            return;
        }

        if(immediatelly)
        {
            Canvas.enabled = isOpening;
        }

        var rectTransform = ContentTransform;
        var animationTime = AnimationTime;
        var isHorizontalDirection = MenuSlideDirection == SlideDirection.Left || MenuSlideDirection == SlideDirection.Right;
        if (isOpening)
        {
            Canvas.enabled = true;
            if (isHorizontalDirection)
                LeanTween.moveX(rectTransform, 0, animationTime).setEaseOutExpo();
            else
                LeanTween.moveY(rectTransform, 0, animationTime).setEaseOutExpo();

            FadeIn();
            return;
        }

        LeanTween.cancel(rectTransform);
        switch (MenuSlideDirection)
        {
            case SlideDirection.Right:
                LeanTween.moveX(rectTransform, -screenWidth, animationTime).setEaseOutExpo().setOnComplete(() => {
                    Canvas.enabled = false;
                });
                break;
            case SlideDirection.Left:
                LeanTween.moveX(rectTransform, screenWidth, animationTime).setEaseOutExpo().setOnComplete(() => {
                    Canvas.enabled = false;
                });
                break;
            case SlideDirection.Down:
                LeanTween.moveY(rectTransform, screenHeight, animationTime).setEaseOutExpo().setOnComplete(() => {
                    Canvas.enabled = false;
                });
                break;
            case SlideDirection.Up:
                LeanTween.moveY(rectTransform, -screenHeight, animationTime).setEaseOutExpo().setOnComplete(() => {
                    Canvas.enabled = false;
                });
                break;
        }

        FadeOut();
    }
}

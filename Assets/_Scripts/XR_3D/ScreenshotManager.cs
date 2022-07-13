using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    public GameObject[] ObjectsToHide;
    public Canvas[] CanvasesToHide;

    [SerializeField]
    private GameObject Blink;
    [SerializeField]
    private GameObject Watermark;

    private string ScreenshotDirectory { get { return Application.persistentDataPath + "/Screenshots"; } }

    private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";


#if PLATFORM_ANDROID
    private static AndroidJavaObject _activity;
    public static AndroidJavaObject Activity
    {
        get
        {
            if (_activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _activity;
        }
    }
#endif

    private void Start()
    {
        //Debug.Log(Application.persistentDataPath);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreen());
    }

    public IEnumerator CaptureScreen()
    {
        // Skip first frame so button tap doesn't cancel model selection when in edit mode
        yield return null;
        var activeCanvases = CanvasesToHide.Where(x => x.enabled).ToArray();

        ToggleCanvases(activeCanvases, false);
        ToggleObjects(false);
        Watermark.SetActive(true);

        string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        string fileName = string.Format("Screenshot{0}.png", timeStamp);

        yield return new WaitForEndOfFrame();

        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var permission = NativeGallery.SaveImageToGallery(tex, "FormaSvetila Screenshots", fileName);
        switch(permission)
        {
            case NativeGallery.Permission.Granted:
                Debug.Log("Screenshot saved: " + fileName);
                break;
            case NativeGallery.Permission.Denied:
                Debug.LogError("Permission denied!");
                break;
            case NativeGallery.Permission.ShouldAsk:
                Debug.LogError("Permission should ask");
                break;
        }

        #region Old screenshot code


        /*
#if UNITY_EDITOR
            Debug.Log("Saving sreenshot, WINDOWS");
            string pathToSave = string.Format("{0}/{1}", ScreenshotDirectory, fileName);
            if (!Directory.Exists(ScreenshotDirectory))
                Directory.CreateDirectory(ScreenshotDirectory);
            ScreenCapture.CaptureScreenshot(pathToSave);
            Debug.Log("Screenshot taken: " + pathToSave);
#endif

#if PLATFORM_ANDROID && !UNITY_EDITOR
        Debug.Log("Saving sreenshot, ANDROID");
        var tex = ScreenCapture.CaptureScreenshotAsTexture();
        using (var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass))
        {
            using (var cr = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                var image = Texture2DToAndroidBitmap(tex);
                var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, image, fileName, "FormaSvetila AR screenshot");
                Debug.Log("Saved to gallery: " + imageUrl);
            }
        }
#elif UNITY_IOS
        Debug.Log("Saving sreenshot, iOS");
  
#endif
        

        */

        #endregion


        StartCoroutine(FlashScreen());

        ToggleCanvases(activeCanvases, true);
        ToggleObjects(true);
        Watermark.SetActive(false);
    }

    private IEnumerator FlashScreen()
    {
        Blink.SetActive(true);
        yield return null;
        var blinkImage = Blink.GetComponent<UnityEngine.UI.Image>();
        LeanTween.value(Blink, (val) => 
        {
            blinkImage.color = new Color(1, 1, 1, val);
        }, blinkImage.color.a, 0.0f, 0.3f)
            .setOnComplete(() => 
        {
            Blink.SetActive(false);
            blinkImage.color = Color.white;
        });
    }

    private void ToggleCanvases(Canvas[] canvases, bool enabled)
    {
        foreach (var canvas in canvases)
            canvas.enabled = enabled;
    }

    private void ToggleObjects(bool active)
    {
        foreach (var obj in ObjectsToHide)
            obj.SetActive(active);
    }

#if PLATFORM_ANDROID
    private static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
    {
        byte[] encoded = texture2D.EncodeToPNG();
        using (var bf = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
        }
    }
#endif
}

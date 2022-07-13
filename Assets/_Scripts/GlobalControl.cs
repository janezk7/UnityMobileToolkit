using Assets._Scripts.Classes;
using Assets.Scripts.Classes;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    [Header("Settings")]
    public UserSettings DefaultSettings;
    // Global app config
    public bool UseExternalAppConfig;
    public string ExternalAppConfigUrl;
    public bool RequireInternetConnection;

    [Header("Managers")]
    public ErrorManager ErrorManager;

    [Header("Debug fields")]
    public bool MockConfigFileFromResources = false;

    // Deep linking
    public bool MockDeepLink;
    public string MockDeepLinkUrl;

    private string deeplinkURL;
    private bool _isAppConfigInitialized = false;
    private bool _isUserSettingsInitialized = false;

    // Properties
    public AppConfiguration AppConfiguration { get; private set; }
    public UserSettings UserSettings { get; private set; }

    // Localization 
    public UnityEngine.Localization.Tables.StringTable CurrentStringTable { get; set; }

    public string VideoCacheDirectory => Application.persistentDataPath + "/Videos";

    public bool IsAppConfigInitialized
    {
        get { return _isAppConfigInitialized; }
        private set { _isAppConfigInitialized = value; }
    }

    public bool IsUserSettingsInitialized
    {
        get { return _isUserSettingsInitialized; }
        private set { _isUserSettingsInitialized = value; }
    }

    #region Images Cache
    private List<(string imageUrl, Sprite sprite)> ImagesCache { get; set; } = new List<(string imageUrl, Sprite sprite)>();
    #endregion

    // TODO refactor to util class
    public static int ConvertAppVersionNumeric(string appVersion)
    {
        var appVersionNumericString = new string(appVersion.Where(c => char.IsDigit(c)).ToArray());
        if (string.IsNullOrEmpty(appVersionNumericString))
            return 0;
        var appVersionNumber = Int32.Parse(appVersionNumericString);
        return appVersionNumber;
    }

    private void Awake()
    {

#if UNITY_IOS
        // iOS sets refreshRate to 30 by default!
        Application.targetFrameRate = 60;
#endif

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        // Initialize localization. Populate available locales
        yield return LocalizationSettings.InitializationOperation;
        InitializeUserSettings();

        yield return StartCoroutine(InitializeApp());
    }

    public void ReInitApp()
    {
        StartCoroutine(InitializeApp());
    }

    /// <summary>
    /// Get config file for app initialization, setup deep linking
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeApp()
    {
        if (ErrorManager == null)
            ErrorManager = FindObjectOfType<ErrorManager>();

        if (RequireInternetConnection && Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogError("No Internet connection!");
            yield return StartCoroutine(ErrorManager.ShowNoConnectionError());
            yield break;
        }

        // TODO refactor. all app config load and handling to seperate method
        if(UseExternalAppConfig)
        {
            if (MockConfigFileFromResources)
            {
                Debug.Log("*****MOCKING CONFIG FILE FROM RESOURCES*******");

                TextAsset jsonConfig = Resources.Load("ConfigFile") as TextAsset;
                AppConfiguration = JsonUtility.FromJson<AppConfiguration>(jsonConfig.text);
            }
            else
            {
                // Get app config file
                var cd = new CoroutineWithData(this, DependancyProvider.Services.ApiService.GetServerJson(ExternalAppConfigUrl));
                yield return cd.coroutine;
                var httpResponse = cd.result as HttpResponse;

                if (!httpResponse.IsSuccessful)
                {
                    Debug.LogError("Error getting config: " + httpResponse.HttpError);
                    var cdConfigError = new CoroutineWithData(this, GetLocalizedString("App_config_get_error"));
                    yield return cdConfigError.coroutine;
                    StartCoroutine(ErrorManager.ShowError((string)cdConfigError.result, reloadApp: true));
                    yield break;
                }

                AppConfiguration = JsonUtility.FromJson<AppConfiguration>(httpResponse.JsonText);
            }

            // Check version 
            var currentAppVersion = ConvertAppVersionNumeric(Application.version);
            Debug.Log("Current version: " + Application.version);
            int latestVersionNumber = ConvertAppVersionNumeric(AppConfiguration.latestVersion);
            int latestMandatoryVersionNumber = ConvertAppVersionNumeric(AppConfiguration.latestMandatoryVersion);
            bool needsUpdate = latestMandatoryVersionNumber > currentAppVersion;
            if (needsUpdate)
            {
                Debug.Log("Needs update! Available mandatory version: " + AppConfiguration.latestMandatoryVersion);
                // Display error and open app url 
                var messageKey = "";
                var appUrl = "";
    #if UNITY_IOS
                Debug.Log("Opening IOS app link: "+ AppConfiguration.iosAppUrl);
                messageKey = "Needs_update_ios";
                appUrl = AppConfiguration.iosAppUrl;
    #elif UNITY_ANDROID
                Debug.Log("Opening Android app link: " + AppConfiguration.androidAppUrl);
                messageKey = "Needs_update_android";
                appUrl = AppConfiguration.androidAppUrl;

    #endif
                var cdMessage = new CoroutineWithData(this, GetLocalizedString(messageKey));
                yield return cdMessage.coroutine;

                var cdUpdate = new CoroutineWithData(this, GetLocalizedString("Update"));
                yield return cdUpdate.coroutine;

                StartCoroutine(ErrorManager.ShowError((string)cdMessage.result, (string)cdUpdate.result, () => Application.OpenURL(appUrl)));
                yield break;
            }

            var apiEndpoint = AppConfiguration.apiEndpoint;
            if(string.IsNullOrEmpty(apiEndpoint))
            {
                Debug.LogError("Missing api endpoint! Check configuration file on : " + ExternalAppConfigUrl);
                var cdError = new CoroutineWithData(this, GetLocalizedString("App_config_error"));
                yield return cdError.coroutine;
                StartCoroutine(ErrorManager.ShowError((string)cdError.result));
                yield break;
            }

            DependancyProvider.Services.ApiService.SetApiEndpoint(AppConfiguration.apiEndpoint);
        }

        // Register deep linking
        Debug.Log("Registering OnDeepLinkActivated");
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (!UserSettings.IsFirstTime)
            HandleDeepLinkOnStartup();

        IsAppConfigInitialized = true;
    }

    //
    /* Deep linking */
    //
    private void OnDeepLinkActivated(string url)
    {
        deeplinkURL = url;
        Debug.Log("TODO: Handle deeplink. Got url: " + url);
        // TODO implement your logic to handle url
    }

    public void HandleDeepLinkOnStartup()
    {
        var applicationAbsoluteUrl = Application.absoluteURL;
        if (MockDeepLink)
            applicationAbsoluteUrl = MockDeepLinkUrl;
        if (!string.IsNullOrEmpty(applicationAbsoluteUrl))
        {
            Debug.Log("Cold starting: " + applicationAbsoluteUrl);
            OnDeepLinkActivated(applicationAbsoluteUrl);
        }
        else
            deeplinkURL = "[none]";
    }

    //
    /* Load methods */
    //

    public IEnumerator LoadAndCacheImage(string imageUrl, int? height, bool cacheImage = true, bool clearImageCache = false)
    {
        if (clearImageCache)
            ClearImageCache();

        var imageCache = ImagesCache.Find(x => x.imageUrl == imageUrl);
        if (imageCache.sprite != null)
        {
            yield return imageCache.sprite;
            yield break;
        }

        var cd = new CoroutineWithData(this, DependancyProvider.Services.ApiService.LoadAndGetImage(imageUrl));
        yield return cd.coroutine;
        var response = cd.result as ApiResponse;
        if(!response.Ok)
        {
            Debug.Log("Error loading image: " + imageUrl);
            yield break;
        }

        var texture = response.Data as Texture2D;
        if(height.HasValue)
        {
            var newHeight = height.Value;
            var newWidth = texture.width * height.Value / texture.height; 
            TextureScale.Bilinear(texture, newWidth, newHeight);
        }
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if (UserSettings.IsCacheImages || cacheImage)
        {
            ImagesCache.Add((imageUrl, sprite));
            if(ImagesCache.Count > UserSettings.MaxImageCache)
                ImagesCache.RemoveAt(0);
        }

        yield return sprite;
    }

    //
    /* Settings methods */
    //
    public void InitializeUserSettings()
    {
        var importedSettings = UserSettingsUtil.ImportUserSettings();
        var isFirstTime = importedSettings == null;

        // TODO: Modify loading logic if needed
        UserSettings = isFirstTime ? ScriptableObject.Instantiate<UserSettings>(DefaultSettings) : importedSettings;
        UserSettings.IsFirstTime = isFirstTime;

        //UserSettings.SaveSettings();
        UserSettings.ApplySettings();

        IsUserSettingsInitialized = true;
        Debug.Log("User settings initialized!");
    }

    #region Cache Management
    public void ClearCache()
    {
        DependancyProvider.Services.AssetService.ClearCache();
        ClearImageCache();
    }

    public void ClearImageCache()
    {
        ImagesCache.Clear();
    }

    #endregion

    private IEnumerator GetLocalizedString(string key)
    {
        var stringOp = new LocalizedString(CurrentStringTable.TableCollectionName, key);
        var locString = stringOp.GetLocalizedString();

        yield return locString;
    }
}
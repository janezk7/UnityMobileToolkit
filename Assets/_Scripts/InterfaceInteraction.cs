using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;

public class InterfaceInteraction : MonoBehaviour
{
    public static InterfaceInteraction Instance;

    [Header("Settings")]
    [SerializeField]
    private bool languageSelectorOnStartup = false;
    [SerializeField]
    private bool isDebugMode = false; // Display debug messages on the DebugText component
    [SerializeField]
    private bool useMockData = false;

    [Header("Managers")]
    public ErrorManager ErrorManager;
    public SearchBarManager SearchBarManager; // Search elements in TopBar
    public SearchViewManager SearchViewManager; // Search overlay with filter and content
    public LanguageSelectorManager LanguageSelectorOverlay;

    [Header("Top Bar")]
    public GameObject SidemenuButton;
    public GameObject BackButton;
    public GameObject SearchButton;
    public TMPro.TextMeshProUGUI ViewTitle;

    [Header("Views")]
    public GameObject CurrentViewObject;
    public GameObject[] ViewsArray;

    [Header("Debug fields")]
    public TMPro.TextMeshProUGUI DebugText;

    // App views TODO: Add your own views
    public enum AppView
    {
        MainMenu = 0,
        Settings = 1,
        ViewNavigation = 2,
        UIUX = 3,
        DataLoading = 4,
        Details = 5,
        SearchFilter = 6,
        Sidemenu = 7,
        ErrorDialog = 8,
        Localization = 9,
        Model3d = 10
        // TODO: Add your views
    };

    // Properties
    public static StringTable CurrentStringTable 
    { 
        get { return GlobalControl.Instance.CurrentStringTable; } 
        private set { GlobalControl.Instance.CurrentStringTable = value; } 
    }

    public AppView CurrentView;
    public Stack<AppView> NavigationHistory { get; private set; }


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Initialize history stack
        NavigationHistory = new Stack<AppView>();
    }

    public IEnumerator Start()
    {
        GlobalControl.Instance.ErrorManager = ErrorManager;

        Debug.Log("Loading scene!");

        if(DebugText != null)
            DebugText.enabled = isDebugMode;

        // WORKFLOW: TODO: UPDATE
        // 1. Global control: starts, loads user settings, waits for InterfaceInteraction
        // 2. Global control: Initializes app, shows connection errors, loads AppConfig file from server, initializes API, checks if update is needed
        // 3.a Interface Interaction: Show LanguageSelector if first time run
        // 3.b LanguageSelector: On select, handle deep link or start app normally

        // Wait for app initialization
        while (!GlobalControl.Instance.IsAppConfigInitialized)
            yield return null;

        if(GlobalControl.Instance.UserSettings.IsFirstTime)
        {
            // Show language select view
            if(languageSelectorOnStartup)
                ToggleLanguageSelector(true);
        }

        LoadStartupPage();
    }

    public void LoadStartupPage()
    {
        NavigateToView(CurrentView);
    }

    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            ToggleLanguageSelector(true);
        }
    }

    public void ShowBackButton()
    {
        ToggleBurgerButton(false);
    }

    public void ShowSidemenuButton()
    {
        ToggleBurgerButton(true);
    }

    /// <summary>
    /// Call current IView NavigateBack implementation. DON'T CALL in IView.NavigateBack() ! Causes StackOverflow.
    /// </summary>
    public void GoBack()
    {
        if (SearchViewManager.IsOverlayShown)
        {
            SearchViewManager.HandleBackNavigation();
            return;
        }

        CurrentViewObject.GetComponent<IView>().NavigateBack();
    }

    /// <summary>
    /// Try to navigate to next view on the history stack
    /// </summary>
    public void NavigatePreviousView()
    {
        if(NavigationHistory.Count == 0)
        {
            // TODO: handle back navigation at root of app
            return;
        }
        
        var previousView = NavigationHistory.Pop();
        NavigateToView(previousView, pushHistory: false);
    }

    public void UpdateDebugText(string text)
    {
        if (!isDebugMode)
            return;

        DebugText.text = text;
    }

    #region Helper Navigation Methods

    public void NavigateToSettings()
    {
        NavigateToView(AppView.Settings);
    }

    public void NavigateToViewAndNavigation()
    {
        NavigateToView(AppView.ViewNavigation);
    }

    public void NavigateToUIUX()
    {
        NavigateToView(AppView.UIUX);
    }
    public void NavigateToDataLoading()
    {
        NavigateToView(AppView.DataLoading);
    }
    public void NavigateToDetails()
    {
        NavigateToView(AppView.Details);
    }
    public void NavigateToSearchFilter()
    {
        NavigateToView(AppView.SearchFilter);
    }
    public void NavigateToSidemenu()
    {
        NavigateToView(AppView.Sidemenu);
    }
    public void NavigateToErrorDialog()
    {
        NavigateToView(AppView.ErrorDialog);
    }
    public void NavigateToLocalization()
    {
        NavigateToView(AppView.Localization);
    }
    public void NavigateToModel3d()
    {
        // TODO: move 3d model viewer. 
    }
    public void NavigateToLanguageSelector()
    {
        LanguageSelectorOverlay.ToggleOverlay(true);
    }
    public void NavigateToExternalNavigation()
    {
        Application.OpenURL("https://www.google.com/search?q=dogs");
        // TODO: Navigate to link
    }

    public void ToggleLanguageSelector(bool show)
    {
        LanguageSelectorOverlay.ToggleOverlay(show);
    }

    #endregion

    /// <summary>
    /// Helper method for safe navigation. Do necessary cleanup or other initialization before calling InitializeAppView
    /// </summary>
    /// <param name="view"></param>
    public void NavigateToView(AppView view, bool pushHistory = true)
    {
        // Default logic on navigation
        ShowBackButton();
        SearchBarManager.ToggleSearchBar(show: false);

        switch (view)
        {
            case AppView.MainMenu:
                // Show sidemenu button instead of back button. Could be also called in IView.Init()
                ShowSidemenuButton();
                break;
            case AppView.SearchFilter:
                SearchBarManager.ToggleSearchBar(show: true);
                break;
            // ....
        }

        InitializeAppView(view, pushHistory);
    }

    /// <summary>
    /// Unsafe for navigation! Use NavigateToView instead
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    private GameObject InitializeAppView(AppView view, bool pushHistory)
    {
        // Disable all views and get selected
        int selectedViewIndex = -1;
        for (int i = 0; i < ViewsArray.Length; i++)
        {
            ViewsArray[i].SetActive(false);
            if (i == (int)view)
                selectedViewIndex = i;
        }

        if(pushHistory)
            NavigationHistory.Push(CurrentView);
        CurrentView = view;
        CurrentViewObject = ViewsArray[selectedViewIndex].gameObject;
        CurrentViewObject.SetActive(true);

        // Dismiss error message
        ErrorManager?.DismissError();

        // Update view title
        UpdateViewTitle();

        // Initialize view
        CurrentViewObject.GetComponent<IView>().InitView();

        return CurrentViewObject;
    }

    private void UpdateViewTitle()
    {
        var viewManager = CurrentViewObject.GetComponent<IView>();
        ViewTitle.text = viewManager?.GetTitle();
    }

    private void ToggleBurgerButton(bool showSidemenu)
    {
        SidemenuButton.SetActive(showSidemenu);
        BackButton.SetActive(!showSidemenu);
    }
}

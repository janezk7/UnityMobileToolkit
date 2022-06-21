using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour, IView
{
    public InterfaceInteraction InterfaceInteraction;
    public Transform CategoryList;
    public LoadingScript LoadingManager;

    [Header("Prefabs")]
    public GameObject CategoryCard;
    public GameObject ListSeperator;

    public string GetTitle() => "Categories";

    public void InitView()
    {
        InterfaceInteraction.ShowBackButton();
    }

    public void NavigateBack()
    {
        // TODO: Implement back function
        InterfaceInteraction.NavigatePreviousView();
    }

    IEnumerator LoadAndPopulate(Category superCategory)
    {
        LoadingManager.BeginLoading();

        yield return StartCoroutine(GlobalControl.Instance.LoadAndCacheCategories());
        yield return StartCoroutine(PopulateCategoryList());

        LoadingManager.EndLoading();
    }

    IEnumerator PopulateCategoryList()
    {
        // Filter categories 
        var categories = GlobalControl.Instance.CategoriesCache;
        if (categories is null)
            yield break;

        // Clear current category cards
        foreach (Transform t in CategoryList)
            Destroy(t.gameObject);

        foreach (var category in categories)
        {
            var categoryCard = Instantiate(CategoryCard, CategoryList);
            categoryCard.GetComponent<CategoryCardManager>().Init(category);
            var button = categoryCard.GetComponent<Button>();

            // TODO: Implement custom list item click handler
            button.onClick.AddListener(() => Debug.Log("TODO: implement button handler"));

            var seperator = Instantiate(ListSeperator, CategoryList);
            yield return null;
        }
    }
}

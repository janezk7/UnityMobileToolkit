using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{
    /// <summary>
    /// Get title of view
    /// </summary>
    /// <returns></returns>
    public string GetTitle();
    /// <summary>
    /// Calls when navigating to view
    /// </summary>
    public void InitView();

    /// <summary>
    /// Calls when calling global back function
    /// </summary>
    public void NavigateBack();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CategoryWrapper
{   
    public Category[] categories;
}

[Serializable]
public class Category
{
    public int Id;
    public string Name;
    public int PictureId;
    public string MimeType;
    public string SeoFilename;
    public string PictureUrl;
    public int CategoryParentId;
    public object CustomProperties;

    public string PictureUrlHttp
    {
        get
        {
            return PictureUrl?.Replace("https", "http");
        }
    }
}

using Assets._Scripts.Services;
using Assets._Scripts.Services.ApiService;
using Assets._Scripts.Services.AssetService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependancyProvider
{
    private static DependancyProvider services;
    public static DependancyProvider Services
    {
        get
        {
            if (services == null)
                services = new DependancyProvider();
            return services;
        }
    }

    public DependancyProvider()
    {
        // Register services here
        ApiService = new ApiService();
        AssetService = new AssetServiceMock();
    }

    // Services
    public IApiService ApiService;
    public IAssetService AssetService;
}

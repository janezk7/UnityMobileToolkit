using Assets._Scripts.Services;
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
        AssetService = new Assets._Scripts.Services.AssetService.AssetServiceMock();
    }

    // Services
    public IAssetService AssetService;
}

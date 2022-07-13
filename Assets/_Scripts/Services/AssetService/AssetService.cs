using System.Collections;
using UnityEngine;
using Assets._Scripts.Entities;
using Assets.Scripts.Util;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes;
using Assets._Scripts.Classes;

namespace Assets._Scripts.Services.AssetService
{
    public interface IAssetService
    {
        public List<Asset> AssetsCache { get; set; }
        public Asset AssetCache { get; set; }
        public void ClearCache();
        public IEnumerator GetAssets(MonoBehaviour monoBehaviour, ApiQueryObject apiQueryObject, bool clearCache = false);
        public IEnumerator GetAssetDetails(MonoBehaviour monoBehaviour, int assetId, bool clearCache = false);
    }

    public class AssetService : IAssetService
    {
        private static AssetService instance;
        public static AssetService Instance
        {
            get
            {
                if (instance is null)
                    instance = new AssetService();
                return instance;
            }
        }

        // Data caches
        public List<Asset> AssetsCache { get; set; }
        public Asset AssetCache { get; set; }

        public AssetService() { }
        ~AssetService() { ClearCache(); }

        public void ClearCache()
        {
            AssetsCache?.Clear();
            AssetCache = null;
        }

        /// <summary>
        /// Load distributor assets
        /// </summary>
        /// <param name="apiQueryObject"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public IEnumerator GetAssets(MonoBehaviour monoBehaviour, ApiQueryObject apiQueryObject, bool clearCache = false)
        {
            // Always clear cache if using a search string
            if (clearCache || apiQueryObject.SearchString != null)
                ClearCache();

            // Check if cache is ok and if it exsits (must check passed filter object first to know if we can use cache)
            bool isCacheOk = true && apiQueryObject.SearchString == null;
            bool itemsCacheExists = AssetsCache != null && isCacheOk;
            if (itemsCacheExists)
            {
                yield return new ApiResponse() { Data = AssetsCache };
                yield break;
            }

            var api = DependancyProvider.Services.ApiService;
            var queryString = apiQueryObject.SearchString;
            var endpoint = string.Format("{0}/{1}&query={2}", api.ApiDomain, "modelProducts", queryString);
            var cd = new CoroutineWithData(monoBehaviour, api.GetHttpResponse(endpoint));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;

            if (!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            var assets = JsonUtility.FromJson<AssetsWrapper>(response.JsonText);

            yield return new ApiResponse() { Data = assets.assets };
        }

        public IEnumerator GetAssetDetails(MonoBehaviour monoBehaviour, int assetId, bool clearCache = false)
        {
            if (clearCache)
                ClearCache();

            bool isCacheOk = AssetCache != null && AssetCache.Id == assetId;
            if (isCacheOk)
            {
                yield return new ApiResponse() { Data = AssetCache };
                yield break;
            }

            var api = DependancyProvider.Services.ApiService;
            var endpoint = string.Format("{0}/{1}?assetId={3}", api.ApiDomain, "assetDetails", assetId);
            var cd = new CoroutineWithData(monoBehaviour, api.GetHttpResponse(endpoint));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;
            if (!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            var asset = JsonUtility.FromJson<Asset>(response.JsonText);
            yield return new ApiResponse() { Data = asset };
        }
    }
}

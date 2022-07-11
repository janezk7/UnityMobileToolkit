using Assets._Scripts.Entities;
using Assets.Scripts.Classes;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Services.AssetService
{
    public class AssetServiceMock : IAssetService
    {
        public List<Asset> AssetsCache { get; set; }

        public void ClearCache()
        {
            AssetsCache?.Clear();
        }

        public IEnumerator GetAssets(MonoBehaviour monoBehaviour, ApiQueryObject apiQueryObject, bool clearCache = false)
        {
            var itemList = new List<Asset>()
            {
                new Asset() { Name = "Item 1", Description = "Sample description" },
                new Asset() { Name = "Item 2", Description = "Sample desctiption" },
                new Asset() { Name = "Item 3", Description = "Sample desctiption" },
                new Asset() { Name = "Item 4", Description = "Sample desctiption" },
                new Asset() { Name = "Item 5", Description = "Sample desctiption" },
                new Asset() { Name = "Item 6", Description = "Sample desctiption" }
            };
            // Mock filtering
            if (!string.IsNullOrEmpty(apiQueryObject.SearchString))
            {
                var ss = apiQueryObject.SearchString.ToLower();
                itemList = itemList.FindAll(x => x.Name.ToLower().Contains(ss));
            }

            yield return new ApiResponse() { Data = itemList };
        }
    }
}

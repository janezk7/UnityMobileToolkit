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
        public Asset AssetCache { get; set; }

        public void ClearCache()
        {
            AssetsCache?.Clear();
            AssetCache = null;
        }

        public IEnumerator GetAssetDetails(MonoBehaviour monoBehaviour, int assetId, bool clearCache = false)
        {
            var asset = new Asset()
            {
                Id = 1,
                Name = "Item 1",
                Description = "Product description...",
                PictureList = new AssetImage[] 
                {
                    new AssetImage() { PictureUrl = "https://cdn-icons-png.flaticon.com/512/616/616554.png" },
                    new AssetImage() { PictureUrl = "https://cdn-icons-png.flaticon.com/512/616/616554.png" },
                    new AssetImage() { PictureUrl = "https://cdn-icons-png.flaticon.com/512/616/616554.png" }
                }
            };

            yield return new ApiResponse() { Data = asset };
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

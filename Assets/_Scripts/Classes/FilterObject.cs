using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes
{
    // Sample Filter object 
    public class FilterObject
    {
        public ApiQueryObject ApiQueryObject { get; private set; } // Filter object to be sent to api
        public ManualFilterObject ManualFilterObject { get; private set; } // Filter object to be used on the client side
        public FilterObject(string searchString, FilterManager.SortType? sortType)
        {
            ApiQueryObject = new ApiQueryObject()
            {
                SearchString = searchString
            };
            ManualFilterObject = new ManualFilterObject()
            {
                SortType = sortType
            };
        }
    }

    public class ApiQueryObject
    {
        // TODO: Add other query fields 
        public string SearchString { get; set; }
    }

    public class ManualFilterObject
    {
        // TODO: Add other query fields 
        public FilterManager.SortType? SortType { get; set; }

        public bool HasFilter => SortType.HasValue;

        public List<T> GetFilteredItems<T>(List<T> items)
        {
            var filteredProducts = new List<T>();
            if (items == null || items.Count == 0)
                return filteredProducts;

            // TODO: Implement filtering/sorting
            if(SortType.HasValue)
            {
                switch(SortType.Value)
                {
                    case FilterManager.SortType.Alphabetical:
                        filteredProducts = items.OrderBy(x => x.ToString()).ToList();
                        break;
                    case FilterManager.SortType.AlphabeticalDesc:
                        filteredProducts = items.OrderByDescending(x => x.ToString()).ToList();
                        break;
                }
            }

            return filteredProducts;
        }
    }
}

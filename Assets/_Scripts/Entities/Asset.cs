using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Entities
{
    [Serializable]
    public class AssetsWrapper
    {
        public Asset[] assets;
    }

    [Serializable]
    public class Asset
    {
        public int Id;
        public string Name;
        public string Description;
        public string ImageUrl;
    }
}

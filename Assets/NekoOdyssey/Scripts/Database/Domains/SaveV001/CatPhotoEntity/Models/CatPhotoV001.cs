using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models
{
    [Serializable]
    public class CatPhotoV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] public string CatCode { get; set; }
        
        [NotNull] public string AssetBundleName { get; set; }

        public CatPhotoV001()
        {
        }

        public CatPhotoV001(string catCode, string assetBundleName)
        {
            CatCode = catCode;
            AssetBundleName = assetBundleName;
        }
    }
}
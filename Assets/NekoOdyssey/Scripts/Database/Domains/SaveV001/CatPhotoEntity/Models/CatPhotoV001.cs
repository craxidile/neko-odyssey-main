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
    }
}
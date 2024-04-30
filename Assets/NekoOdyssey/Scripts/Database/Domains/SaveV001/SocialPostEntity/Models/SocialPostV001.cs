using System;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models
{
    [Serializable]
    public class SocialPostV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(CatPhotoV001))]
        public int CatPhotoId { get; set; }
        
        [NotNull] public int LikeCount { get; set; }
    }
}
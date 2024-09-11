using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;
using UnityEngine;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models
{
    [Serializable]
    public class SocialPostV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(CatPhotoV001))]
        public int CatPhotoV001Id { get; set; }
        
        [NotNull] public int LikeCount { get; set; }
        
        [Ignore] public CatPhotoV001 Photo { get; set; }

        public SocialPostV001()
        {
        }

        public SocialPostV001(CatPhotoV001 catPhoto)
        {
            CatPhotoV001Id = catPhoto.Id;
            Photo = catPhoto;
        }
    }
}
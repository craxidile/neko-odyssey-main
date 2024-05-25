using System;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureLikeEntity.Models
{
    [Serializable]
    public class SocialFutureLikeV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(SocialPostV001))]
        public int SocialPostId { get; set; }
        
        [NotNull] public int LikeCount { get; set; }
        
        [NotNull] public float ExpCdfLambda { get; set; }
        
        [NotNull] public long Round { get; set; }

        public SocialFutureLikeV001()
        {
        }

        public SocialFutureLikeV001(int socialPostId, int likeCount, float expCdfLambda)
        {
            SocialPostId = socialPostId;
            LikeCount = likeCount;
            ExpCdfLambda = expCdfLambda;
        }
    }
}
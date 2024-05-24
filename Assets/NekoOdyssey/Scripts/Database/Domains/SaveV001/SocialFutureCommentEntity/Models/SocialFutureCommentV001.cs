using System;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureLikeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureCommentEntity.Models
{
    [Serializable]
    public class SocialFutureCommentV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(SocialPostV001))]
        public int SocialPostId { get; set; }
        
        [NotNull] public int CommentCount { get; set; }
        
        [NotNull] public float ExpCdfLambda { get; set; }
        
        [NotNull] public long Round { get; set; }

        public SocialFutureCommentV001()
        {
        }

        public SocialFutureCommentV001(int socialPostId, int commentCount, float expCdfLambda)
        {
            SocialPostId = socialPostId;
            CommentCount = commentCount;
            ExpCdfLambda = expCdfLambda;
        }
    }
}
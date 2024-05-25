using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Social.Entities.SocialPostTemplateEntity.Models
{
    [Serializable]
    public class SocialPostTemplate : EntityBase<int>, IAggregateRoot
    {
        [NotNull] public string AssetBundleName { get; set; }
        
        [NotNull] public float ExpCdfLambda { get; set; }
        
        [NotNull] public int FinalLikeCount { get; set; }
        
        [NotNull] public int FinalCommentCount { get; set; }
        
        public SocialPostTemplate()
        {
        }
    }
}
using System;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialCommentEntity.Models
{
    [Serializable]
    public class SocialCommentV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(SocialPostV001))]
        public int SocialPostId { get; set; }
        
        [NotNull] public string FriendCode { get; set; }
        
        [NotNull] public string CommentCode { get; set; }
    }
}
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Models
{
    public class SiteScene : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(SiteEntity.Models.Site))]
        public int SiteId { get; set; }

        [Ignore] public virtual SiteEntity.Models.Site Site { get; set; }

        public string SiteName { get; set; }

        [NotNull] public string Name { get; set; }
        
        public string ActiveGameObject { get; set; }

        public SiteScene()
        {
        }

        public SiteScene(int siteId, string name)
        {
            SiteId = siteId;
            Name = name;
        }

        public SiteScene(int siteId, string siteName, string name)
        {
            SiteId = siteId;
            SiteName = siteName;
            Name = name;
        }
    }
}
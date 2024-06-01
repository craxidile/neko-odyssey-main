using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Social.Entities.SocialPostTemplateEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Social.Entities.SocialPostTemplateEntity.Repo
{
    public class SocialPostTemplateRepo : Repository<SocialPostTemplate, int, SQLiteConnection>
    {
        
        public SocialPostTemplateRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<SocialPostTemplate> List()
        {
            return _dbContext.Context.Table<SocialPostTemplate>()
                .OrderByDescending(ct => ct.Id)
                .ToList();
        }

        public SocialPostTemplate FindByAssetBundleName(string name)
        {
            return _dbContext.Context.Table<SocialPostTemplate>()
                .FirstOrDefault(sp => sp.AssetBundleName == name);
        }
    }
}
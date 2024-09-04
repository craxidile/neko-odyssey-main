using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Repo
{
    public class PlayerSiteV001Repo : Repository<PlayerSiteV001, int, SQLiteConnection>
    {
        public PlayerSiteV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<PlayerSiteV001> List()
        {
            return _dbContext.Context
                .Table<PlayerSiteV001>()
                .OrderBy(ps => ps.LastVisit)
                .ToList();
        }

        public PlayerSiteV001 FindBySiteCode(string siteCode)
        {
            return _dbContext.Context
                .Table<PlayerSiteV001>()
                .FirstOrDefault(ps => ps.SiteCode == siteCode);
        }
    }
}
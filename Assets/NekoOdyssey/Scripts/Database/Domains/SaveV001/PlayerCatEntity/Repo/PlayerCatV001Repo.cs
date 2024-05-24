using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Repo
{
    public class PlayerCatV001Repo: Repository<PlayerCatV001, int, SQLiteConnection>
    {
        public PlayerCatV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<PlayerCatV001> List()
        {
            return _dbContext.Context.Table<PlayerCatV001>()
                .OrderBy(i => i.CatCode)
                .ToList();
        }

        public PlayerCatV001 FindByCatCode(string catCode)
        {
            return _dbContext.Context.Table<PlayerCatV001>()
                .FirstOrDefault(i => i.CatCode == catCode);
        }

        public PlayerCatV001 Add(PlayerCatV001 playerCat)
        {
            return base.Add(playerCat);
        }

        public PlayerCatV001 Update(PlayerCatV001 playerCat)
        {
            return base.Update(playerCat);
        }
    }
}
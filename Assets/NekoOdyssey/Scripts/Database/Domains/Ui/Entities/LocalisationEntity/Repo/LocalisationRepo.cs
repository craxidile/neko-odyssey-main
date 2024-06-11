using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Ui.Entities.LocalisationEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Ui.Entities.LocalisationEntity.Repo
{
    public class LocalisationRepo : Repository<Localisation, int, SQLiteConnection>
    {
        public LocalisationRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public Localisation FindByOriginal(string original)
        {
            return _dbContext.Context
                .Table<Localisation>()
                .FirstOrDefault(l => l.Original == original);
        }
    }
}
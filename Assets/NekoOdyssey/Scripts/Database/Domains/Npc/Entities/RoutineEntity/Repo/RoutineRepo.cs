using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Repo
{
    public class RoutineRepo : Repository<Routine, int, SQLiteConnection>
    {
        public RoutineRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<Routine> List()
        {
            return _dbContext.Context.Table<Routine>()
                .OrderBy(r => r.Id)
                .ToList();
        }

        public Routine FindById(int id)
        {
            return _dbContext.Context.Table<Routine>()
                .FirstOrDefault(r => r.Id == id);
        }

        public Routine FindByCode(string code)
        {
            return _dbContext.Context.Table<Routine>()
                .FirstOrDefault(r => r.Code == code);
        }
    }
}
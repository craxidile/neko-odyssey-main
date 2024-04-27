using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Quests
{
    public class NpcDbContext: BaseDbContext
    {
        public NpcDbContext(DbContextOptions options) : base(DatabaseConstants.DbRoutinesQuestsName, options)
        {
        }
    }
}
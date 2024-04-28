using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Npc
{
    public class NpcDbContext: BaseDbContext
    {
        public NpcDbContext(DbContextOptions options) : base(DatabaseConstants.DbNpcName, options)
        {
        }
    }
}
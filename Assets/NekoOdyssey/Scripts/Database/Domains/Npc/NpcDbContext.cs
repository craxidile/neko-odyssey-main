using NekoOdyssey.Constants;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Npc
{
    public class NpcDbContext: BaseDbContext
    {
        public NpcDbContext(DbContextOptions options) : base(DatabaseConstants.DbNpcName, options)
        {
        }
    }
}
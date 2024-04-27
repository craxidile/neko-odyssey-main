using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Quests
{
    public class QuestsDbContext: BaseDbContext
    {
        public QuestsDbContext(DbContextOptions options) : base(DatabaseConstants.DbQuestsName, options)
        {
        }
    }
}
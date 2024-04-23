using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Items
{
    public class ItemsDbContext: BaseDbContext
    {
        public ItemsDbContext(DbContextOptions options): base(DatabaseConstants.DbItemsName, options)
        {
        }
    }
}
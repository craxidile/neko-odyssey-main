using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Cats
{
    public class CatsDbContext: BaseDbContext
    {
        public CatsDbContext(DbContextOptions options): base(DatabaseConstants.DbCatsName, options)
        {
        }
    }
}
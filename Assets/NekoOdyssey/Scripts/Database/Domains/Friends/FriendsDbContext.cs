using NekoOdyssey.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Friends
{
    public class FriendsDbContext : BaseDbContext
    {
        public FriendsDbContext(DbContextOptions options): base(DatabaseConstants.DbFriendsName, options)
        {
        }
    }
}
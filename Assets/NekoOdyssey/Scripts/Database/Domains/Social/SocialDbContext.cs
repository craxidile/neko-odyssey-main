using NekoOdyssey.Constants;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Social
{
    public class SocialDbContext : BaseDbContext
    {
        public SocialDbContext(DbContextOptions options): base(DatabaseConstants.DbSocialName, options)
        {
        }
    }
}
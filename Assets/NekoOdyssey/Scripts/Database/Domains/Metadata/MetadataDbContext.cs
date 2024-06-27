using NekoOdyssey.Constants;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Metadata
{
    public class MetadataDbContext : BaseDbContext
    {
        public MetadataDbContext(DbContextOptions options) : base(DatabaseConstants.DbMetadataName, options)
        {
        }
    }
}
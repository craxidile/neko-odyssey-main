using NekoOdyssey.Constants;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001
{
    public class SaveV001DbContext: BaseDbContext
    {
        public SaveV001DbContext(DbContextOptions options): base(DatabaseConstants.DbSaveVersion001Name, options)
        {
        }
    }
}
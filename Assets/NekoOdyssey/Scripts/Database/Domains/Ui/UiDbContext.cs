using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Domains.Ui
{
    public class UiDbContext : BaseDbContext
    {
        public UiDbContext(DbContextOptions options) : base(DatabaseConstants.DbUiName, options)
        {
        }
    }
}
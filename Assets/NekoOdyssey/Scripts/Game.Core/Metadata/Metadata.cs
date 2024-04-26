using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Metadata;

namespace NekoOdyssey.Scripts.Game.Core.Metadata
{
    public class Metadata
    {
        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            using (new MetadataDbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false })) ;
        }
    }
}
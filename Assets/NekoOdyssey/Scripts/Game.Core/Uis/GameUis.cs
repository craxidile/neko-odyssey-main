using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Ui;
using NekoOdyssey.Scripts.Game.Core.Uis.Localisation;

namespace NekoOdyssey.Scripts.Game.Core.Uis
{
    public class GameUis
    {
        public UiLocalisation Localisation { get; } = new();
        
        public void Bind()
        {
            InitializeDatabase();
            Localisation.Bind();
        }

        public void Start()
        {
            Localisation.Start();
        }

        public void Unbind()
        {
            Localisation.Unbind();
        }
        
        private void InitializeDatabase()
        {
            using (new UiDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }
    }
}
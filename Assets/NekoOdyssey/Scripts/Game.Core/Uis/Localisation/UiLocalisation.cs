using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Ui;
using NekoOdyssey.Scripts.Database.Domains.Ui.Entities.LocalisationEntity.Repo;

namespace NekoOdyssey.Scripts.Game.Core.Uis.Localisation
{
    public class UiLocalisation
    {
        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public string TranslateCurrent(string original)
        {
            return Translate(original, GameRunner.Instance.Core.Settings.Locale);
        }

        public string Translate(string original, Locale locale)
        {
            using (var dbContext = new UiDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new LocalisationRepo(dbContext);
                var localisation = repo.FindByOriginal(original);
                return localisation?.LocalizedText?.ToLocalizedString(locale);
            }
        }
    }
}
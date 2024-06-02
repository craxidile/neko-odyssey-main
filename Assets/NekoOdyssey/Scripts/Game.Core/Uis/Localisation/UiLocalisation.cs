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

        public string Translate(string original, Locale locale)
        {
            using (var dbContext = new UiDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new LocalisationRepo(dbContext);
                var localisation = repo.FindByOriginal(original);
                if (localisation == null) return null;
                switch (locale)
                {
                    case Locale.Th:
                        return localisation.TextTh;
                    case Locale.En:
                        return localisation.TextEn;
                    case Locale.ZhCn:
                        return localisation.TextZhCn;
                    case Locale.ZhTw:
                        return localisation.TextZhTw;
                    case Locale.Ja:
                        return localisation.TextJa;
                    default:
                        return null;
                }
            }
        }
    }
}
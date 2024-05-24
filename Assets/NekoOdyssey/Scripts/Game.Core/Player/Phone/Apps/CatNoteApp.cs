using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Repo;
using NekoOdyssey.Scripts.Models;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class CatNoteApp
    {
        public ICollection<PlayerCatV001> PlayerCats { get; private set; }

        public Subject<ICollection<PlayerCatV001>> OnChangePlayerCats { get; } = new();

        public void Bind()
        {
            LoadPlayerCats();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public bool IsCatCollected(string catCode) => PlayerCats.Any(pc => pc.CatCode == catCode);

        private void LoadPlayerCats()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerCatV001Repo(dbContext);
                PlayerCats = new List<PlayerCatV001>(repo.List());
            }

            OnChangePlayerCats.OnNext(PlayerCats);
        }

        private static PlayerCatV001 GetPlayerCatByCode(PlayerCatV001Repo repo, string catCode)
        {
            var playerCat = repo.FindByCatCode(catCode);
            return playerCat != null ? playerCat : repo.Add(new PlayerCatV001(catCode));
        }

        public void AddCatPetting(string catCode)
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerCatV001Repo(dbContext);
                var playerCat = GetPlayerCatByCode(repo, catCode);
                playerCat.DailyFeedCount++;
                repo.Update(playerCat);
            }

            LoadPlayerCats();
        }

        public void AddCatCapture(string catCode)
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerCatV001Repo(dbContext);
                var playerCat = GetPlayerCatByCode(repo, catCode);
                playerCat.CaptureCount++;
                repo.Update(playerCat);
            }

            LoadPlayerCats();
        }
    }
}
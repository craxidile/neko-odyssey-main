using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Repo;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class SocialNetworkApp
    {
        public ICollection<SocialPostV001> Posts { get; private set; }

        public Subject<ICollection<SocialPostV001>> OnChangeFeeds { get; } = new();

        public GameObject GameObject { get; set; }

        public void Add(string catCode)
        {
            var catPhoto = new CatPhotoV001(catCode, catCode);
            var postList = Posts as List<SocialPostV001>;
            postList?.Insert(0, new SocialPostV001(catPhoto));
            OnChangeFeeds.OnNext(Posts);
        }

        public void Bind()
        {
            LoadPosts();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void RefreshPost(SocialPostV001 post)
        {
            OnChangeFeeds.OnNext(Posts);
        }

        private void LoadPosts()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var socialPostRepo = new SocialPostV001Repo(dbContext);
                Posts = new List<SocialPostV001>(socialPostRepo.List());
            }
            OnChangeFeeds.OnNext(Posts);
        }
    }
}
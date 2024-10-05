using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class ChatboxApp
    {
        public void Bind()
        {
            LoadChatBoxs();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }
        private void LoadChatBoxs()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                //var socialPostRepo = new SocialPostV001Repo(dbContext);
                //Posts = new List<SocialPostV001>(socialPostRepo.List());
            }
            //OnChangeFeeds.OnNext(Posts);
            //GameRunner.Instance.Core.Player.UpdateTotalLikeCount();
        }

        public void Add(string chatId)
        {
            //var catPhoto = new CatPhotoV001(catCode, catCode);
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                //var catPhotoRepo = new CatPhotoV001Repo(dbContext);
                //var dbCatPhoto = catPhotoRepo.FindByAssetBundleName(catCode);
                //catPhoto = dbCatPhoto == null ? catPhotoRepo.Add(catPhoto) : dbCatPhoto;
                //var socialPostRepo = new SocialPostV001Repo(dbContext);
                //socialPostRepo.Add(new SocialPostV001(catPhoto));
            });

            //LoadPosts();
            LoadChatBoxs();
        }
    }
}

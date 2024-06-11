using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Repo;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class PhotoGalleryApp
    {
        public ICollection<CatPhotoV001> Photos { get; private set; }

        public Subject<ICollection<CatPhotoV001>> OnChangePhotos { get; } = new();

        public GameObject GameObject { get; set; }

        public void Bind()
        {
            LoadPhotos();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        private void LoadPhotos()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new CatPhotoV001Repo(dbContext);
                Photos = new List<CatPhotoV001>(repo.List());
            }

            OnChangePhotos.OnNext(Photos);
        }

        public void Add(CatPhotoV001 photo)
        {
            // var photoList = Photos as List<CatPhotoV001>;
            // photoList?.Insert(0, photo);
            // OnChangePhotos.OnNext(Photos);
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var repo = new CatPhotoV001Repo(dbContext);
                var catPhoto = repo.FindByAssetBundleName(photo.CatCode);
                if (catPhoto != null) return;
                repo.Add(photo);
            });
            LoadPhotos();
            GameRunner.Instance.Core.Simulators.SocialNetworkSimulator.Add(photo.CatCode);
        }
    }
}
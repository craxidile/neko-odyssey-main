using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureCommentEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureLikeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Social;
using NekoOdyssey.Scripts.Database.Domains.Social.Entities.SocialPostTemplateEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Social.Entities.SocialPostTemplateEntity.Repo;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NekoOdyssey.Scripts.Game.Core.Simulators.SocialNetwork
{
    public class SocialNetworkSimulator
    {
        private const int SimulationInterval = 60000;

        public List<SocialFutureLikeV001> FutureLikes { get; } = new();
        public List<SocialFutureCommentV001> FutureComments { get; } = new();

        public void Bind()
        {
            using (new SocialDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }

        public void Start()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(SimulationInterval))
                .Subscribe(HandleTimeElapsed)
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }

        public void Add(string assetBundleName)
        {
            var catCode = assetBundleName.Substring(0, 3);
            SocialPostTemplate template;
            using (var dbContext = new SocialDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var templateRepo = new SocialPostTemplateRepo(dbContext);
                template = templateRepo.FindByAssetBundleName(assetBundleName);
            }

            if (template == null) return;

            SocialPostV001 socialPost = null;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var catPhotoRepo = new CatPhotoV001Repo(dbContext);
                var catPhoto = catPhotoRepo.FindByAssetBundleName(assetBundleName);
                if (catPhoto == null) catPhoto = catPhotoRepo.Add(new CatPhotoV001(catCode, assetBundleName));

                var socialPostRepo = new SocialPostV001Repo(dbContext);
                socialPost = socialPostRepo.FindByCatPhotoId(catPhoto.Id);
                if (socialPost == null) socialPost = socialPostRepo.Add(new SocialPostV001(catPhoto));
            }

            if (socialPost == null) return;

            StoreFutureLikes(new SocialFutureLikeV001(socialPost.Id, template.FinalLikeCount, template.ExpCdfLambda));
            StoreFutureComments(
                new SocialFutureCommentV001(socialPost.Id, template.FinalCommentCount, template.ExpCdfLambda)
            );
        }

        private void StoreFutureLikes(SocialFutureLikeV001 likes)
        {
            FutureLikes.Add(likes);
        }

        private void StoreFutureComments(SocialFutureCommentV001 comments)
        {
            FutureComments.Add(comments);
        }

        private int GenerateExpRandom(float expCdfLambda, long round, int max)
        {
            // For random: -ln(1 - (1 - exp(-λ)) * U) / λ
            var maxRandom = Math.Max(2f, Mathf.Floor(max * expCdfLambda * Mathf.Exp(-expCdfLambda * round)));
            return Math.Min(max, (int)Random.Range(0, maxRandom));
        }

        private void HandleTimeElapsed(long ticks)
        {
            var posts = GameRunner.Instance.Core.Player.Phone.SocialNetwork.Posts;
            
            var futureLikesToRemove = new List<SocialFutureLikeV001>();
            foreach (var futureLike in FutureLikes)
            {
                var round = (int)Math.Floor(ticks / 2f);
                var currentRound = round + futureLike.Round;
                var likeCount = GenerateExpRandom(futureLike.ExpCdfLambda, currentRound, futureLike.LikeCount);
                futureLike.Round = currentRound;
                futureLike.LikeCount -= likeCount;
                if (futureLike.LikeCount == 0) futureLikesToRemove.Add(futureLike);

                var post = posts.FirstOrDefault(p => p.Id == futureLike.SocialPostId);
                if (post == null) continue;

                post.LikeCount += likeCount;
                Debug.Log($">>like_count<< {post.LikeCount}");
                GameRunner.Instance.Core.Player.Phone.SocialNetwork.RefreshPost(post);
            }

            var totalLikeCount = posts.Sum(p => p.LikeCount);
            GameRunner.Instance.Core.Player.SetLikeCount(totalLikeCount);
            
            FutureLikes.RemoveAll(fl => futureLikesToRemove.Contains(fl));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Repo;
using NekoOdyssey.Scripts.Game.Core.Player.Bag;
using NekoOdyssey.Scripts.Game.Core.Player.Capture;
using NekoOdyssey.Scripts.Game.Core.Player.Conversation;
using NekoOdyssey.Scripts.Game.Core.Player.ItemObain;
using NekoOdyssey.Scripts.Game.Core.Player.ConfirmationPanel;
using NekoOdyssey.Scripts.Game.Core.Player.Feed;
using NekoOdyssey.Scripts.Game.Core.Player.Petting;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps;
using NekoOdyssey.Scripts.Game.Core.Player.Stamina;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Core.Player
{
    public class Player
    {
        private static bool _finalSceneLoaded = false;

        public PlayerMode Mode { get; private set; } = PlayerMode.Move;
        public PlayerMode PreviousMode { get; private set; } = PlayerMode.Move;
        public bool Running { get; private set; } = false;
        public Vector3 Position { get; private set; }

        public PlayerPhone Phone { get; } = new();
        public PlayerBag Bag { get; } = new();
        public PlayerCapture Capture { get; } = new();
        public PlayerPetting Petting { get; } = new();
        public PlayerFeed Feed { get; } = new();
        public PlayerConversation Conversation { get; } = new();
        public PlayerStamina Stamina { get; } = new(); // linias added
        public PlayerItemObtainPopUp ItemObtainPopUp { get; } = new();
        public PlayerConfirmationPanel ConfirmationPanel { get; } = new();

        // public int Stamina { get; private set; }
        public int PocketMoney { get; private set; }
        public bool DemoFinished { get; private set; }
        public int LikeCount { get; private set; }
        public int FollowerCount { get; private set; }
        public int DayCount => LoadPlayerProperties().DayCount;

        public Tuple<int, int> Time
        {
            get
            {
                var properties = LoadPlayerProperties();
                return Tuple.Create(properties.CurrentHour, properties.CurrentMinute);
            }
        }

        public GameObject GameObject { get; set; }

        public Subject<PlayerMode> OnChangeMode { get; } = new();
        public Subject<bool> OnRun { get; } = new();
        public Subject<Vector2> OnMove { get; } = new();
        public Subject<Vector3> OnChangePosition { get; } = new();
        public Subject<Unit> OnShakeHead { get; } = new();
        public Subject<int> OnChangePocketMoney { get; } = new();
        public Subject<int> OnChangeLikeCount { get; } = new();
        public Subject<int> OnChangeFollowerCount { get; } = new();
        public Subject<Unit> OnFinishDemo { get; } = new();

        public void Bind()
        {
            Phone.Bind();
            Bag.Bind();
            Capture.Bind();
            Conversation.Bind();
            Petting.Bind();
            Feed.Bind();
            Stamina.Bind();
            ItemObtainPopUp.Bind();
            ConfirmationPanel.Bind();
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnPhoneTriggerred
                .Subscribe(_ => SetPhoneMode())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnBagTriggerred
                .Subscribe(_ => SetBagMode())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnMove
                .Subscribe(HandleMove)
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnSpeedStart
                .Subscribe(_ => HandleSpeedStart())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnSpeedEnd
                .Subscribe(_ => HandleSpeedEnd())
                .AddTo(GameRunner.Instance);

            Phone.Start();
            Bag.Start();
            Capture.Start();
            Conversation.Start();
            Petting.Start();
            Feed.Start();

            Stamina.Start();
            Stamina.OnChangeStamina
                .Subscribe(_ => SavePlayerProperties())
                .AddTo(GameRunner.Instance);

            OnChangePocketMoney
                .Subscribe(_ => SavePlayerProperties())
                .AddTo(GameRunner.Instance);

            if (GameRunner.Instance.Core.SaveReady)
                HandleGameSaveReady(default);
            else
                GameRunner.Instance.Core.OnSaveReady
                    .Subscribe(HandleGameSaveReady)
                    .AddTo(GameRunner.Instance);

            ItemObtainPopUp.Start();
            ConfirmationPanel.Start();
        }

        public void Unbind()
        {
            Phone.Unbind();
            Bag.Unbind();
            Capture.Unbind();
            Conversation.Unbind();
            Petting.Unbind();
            Feed.Unbind();
            Stamina.Unbind();
            ItemObtainPopUp.Unbind();
            ConfirmationPanel.Unbind();
        }

        private PlayerPropertiesV001 LoadPlayerProperties()
        {
            PlayerPropertiesV001 playerProperties;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var playerPropertiesRepo = new PlayerPropertiesV001Repo(dbContext);
                playerProperties = playerPropertiesRepo.Load();
                //AddStamina(playerProperties.Stamina);
                Stamina.SetStamina(playerProperties.Stamina);
                PocketMoney = playerProperties.PocketMoney;
                UpdateFollowerCount(playerProperties.LikeCount);
                DemoFinished = playerProperties.DemoFinished;
            }

            return playerProperties;
        }

        private void SavePlayerProperties()
        {
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var playerPropertiesRepo = new PlayerPropertiesV001Repo(dbContext);
                var playerProperties = playerPropertiesRepo.Load();
                playerProperties.Stamina = Stamina.Stamina;
                playerProperties.LikeCount = LikeCount;
                playerProperties.FollowerCount = FollowerCount;
                playerProperties.PocketMoney = PocketMoney;
                playerPropertiesRepo.Update(playerProperties);
            });
        }

        private void ResetPlayerSubmenu()
        {
            if (Mode != PlayerMode.Submenu) return;
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(0);
        }

        public void SetPhoneMode()
        {
            ResetPlayerSubmenu();
            if (Mode != PlayerMode.Move && Mode != PlayerMode.Phone) return;
            Mode = Mode == PlayerMode.Move ? PlayerMode.Phone : PlayerMode.Move;
            OnChangeMode.OnNext(Mode);
        }

        public void SetBagMode()
        {
            ResetPlayerSubmenu();
            if ((Mode != PlayerMode.Move && Mode != PlayerMode.OpenBag) || Bag.Animating) return;
            Mode = Mode == PlayerMode.Move ? PlayerMode.OpenBag : PlayerMode.Stop;
            OnChangeMode.OnNext(Mode);
            // switch (Mode)
            // {
            //     case PlayerMode.Move:
            //         SetMode(PlayerMode.OpenBag);
            //         break;
            //     case PlayerMode.OpenBag:
            //         SetMode(PlayerMode.CloseBag);
            //         break;
            // }
        }

        private void HandleMove(Vector2 input)
        {
            if (Mode != PlayerMode.Move)
            {
                OnMove.OnNext(new Vector2(0, 0));
                return;
            }

            OnMove.OnNext(input);
        }

        private void HandleSpeedStart()
        {
            if (Running) return;
            OnRun.OnNext(Running = true);
        }

        private void HandleSpeedEnd()
        {
            if (!Running) return;
            OnRun.OnNext(Running = false);
        }

        private void UpdateProperties(Action<PlayerPropertiesV001> action)
        {
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var repo = new PlayerPropertiesV001Repo(dbContext);
                var playerProperties = repo.Load();
                action(playerProperties);
                repo.Update(playerProperties);
            });
            LoadPlayerProperties();
        }

        private void HandleGameSaveReady(Unit _)
        {
            LoadPlayerProperties();
        }

        public void SetMode(PlayerMode mode)
        {
            Debug.Log($"<color=green>>>change_mode<< {mode} {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name}</color>");
            PreviousMode = Mode;
            Mode = mode;
            OnChangeMode.OnNext(Mode);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            OnChangePosition.OnNext(position);
        }

        public void SetLikeCount(int likeCount)
        {
            LikeCount = likeCount;
            OnChangeLikeCount.OnNext(LikeCount);
            UpdateFollowerCount(likeCount);
        }

        public void UpdateTotalLikeCount()
        {
            var totalLikeCount = Phone.SocialNetwork.Posts.Sum(p => p.LikeCount);
            SetLikeCount(totalLikeCount);
        }

        public void UpdateFollowerCount(int likeCount)
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var playerPropertiesRepo = new PlayerPropertiesV001Repo(dbContext);
                var playerProperties = playerPropertiesRepo.Load();
                DemoFinished = playerProperties.DemoFinished;
            }

            FollowerCount = (int)Math.Floor(.05f * likeCount);
            OnChangeFollowerCount.OnNext(FollowerCount);
            // SavePlayerProperties();
            Debug.Log($">>follower_count<< {FollowerCount}");
            if (!_finalSceneLoaded && !DemoFinished && FollowerCount >= 100)
            {
                _finalSceneLoaded = true;
                Debug.Log($">>load_final<<");

                //GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
                //{/
                //    var repo = new PlayerPropertiesV001Repo(dbContext);
                //    var properties = repo.Load();
                //    properties.DemoFinished = true;
                //    OnFinishDemo.OnNext(default);
                //    repo.Update(properties);
                //});

                //DOVirtual.DelayedCall(2f, () =>
                //{
                //    GameRunner.Instance.Core.GameScene.CloseScene();
                //    DOVirtual.DelayedCall(4f,
                //        () => { SiteRunner.Instance.Core.Site.SetSite("NekoInside28BedroomFinal"); });
                //});

                OnFinishDemo.OnNext(default);
            }
        }

        // public void AddStamina(int addition)
        // {
        //     Stamina = Math.Min(AppConstants.Stamina_Max, AppConstants.Stamina + addition);
        //     OnChangeStamina.OnNext(Stamina);
        //     SavePlayerProperties();
        // }

        public void UpdateDayCount(int dayCount) => UpdateProperties(properties => properties.DayCount = dayCount);

        public int GetDayCount()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerPropertiesV001Repo(dbContext);
                var properties = repo.Load();
                return properties.DayCount;
            }
        }

        public void UpdateTime(int hour, int minute)
        {
            UpdateProperties(properties =>
            {
                properties.CurrentHour = hour;
                properties.CurrentMinute = minute;
            });
        }

        public Tuple<int, int> GetTime()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerPropertiesV001Repo(dbContext);
                var properties = repo.Load();
                return Tuple.Create(properties.CurrentHour, properties.CurrentMinute);
            }
        }

        public void AddAchievedQuest(string questCode)
        {
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var repo = new PlayerQuestV001Repo(dbContext);
                var playerQuest = repo.FindByQuestCode(questCode);
                if (playerQuest != null) return;
                repo.Add(new PlayerQuestV001(questCode));
            });
        }

        public bool IsQuestComplete(string questCode)
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerQuestV001Repo(dbContext);
                return repo.FindByQuestCode(questCode) != null;
            }
        }

        public void AddPocketMoney(int money)
        {
            PocketMoney += money;
            OnChangePocketMoney.OnNext(PocketMoney);
        }
        
        public void UsePocketMoney(int money)
        {
            PocketMoney -= money;
            OnChangePocketMoney.OnNext(PocketMoney);
        }
        
        public void MarkSiteAsVisited(string siteName)
        {
            var now = DateTime.Now;
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var repo = new PlayerSiteV001Repo(dbContext);
                var playerSite = repo.FindBySiteCode(siteName);
                if (playerSite != null)
                {
                    playerSite.LastVisit = now;
                    repo.Update(playerSite);
                }
                else
                {
                    repo.Add(new PlayerSiteV001()
                    {
                        SiteCode = siteName,
                        LastVisit = now,
                    });
                }
            });
        }

        public bool IsSiteVisited(string siteName)
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerSiteV001Repo(dbContext);
                var playerSite = repo.FindBySiteCode(siteName);
                return playerSite != null;
            }
        }

        public void ShakeHead()
        {
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);
            OnShakeHead.OnNext(default);
        }

        public void FinishHeadShake()
        {
            GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteNameActive();
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(0);
        }
    }
}
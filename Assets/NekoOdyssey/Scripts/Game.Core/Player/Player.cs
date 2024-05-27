using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Repo;
using NekoOdyssey.Scripts.Game.Core.Player.Bag;
using NekoOdyssey.Scripts.Game.Core.Player.Capture;
using NekoOdyssey.Scripts.Game.Core.Player.Conversation;
using NekoOdyssey.Scripts.Game.Core.Player.Petting;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player
{
    public class Player
    {
        private static bool _initialized;

        public PlayerMode PreviousMode { get; private set; } = PlayerMode.Move;
        public PlayerMode Mode { get; private set; } = PlayerMode.Move;
        public bool Running { get; private set; } = false;
        public Vector3 Position { get; private set; }

        public PlayerPhone Phone { get; } = new();
        public PlayerBag Bag { get; } = new();
        public PlayerCapture Capture { get; } = new();
        public PlayerPetting Petting { get; } = new();
        public PlayerConversation Conversation { get; } = new();
        public SaveV001DbWriter SaveDbWriter { get; } = new();

        public int Stamina { get; private set; }
        public int PocketMoney { get; private set; }
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
        public Subject<int> OnChangeStamina { get; } = new();
        public Subject<int> OnChangePocketMoney { get; } = new();
        public Subject<int> OnChangeLikeCount { get; } = new();
        public Subject<int> OnChangeFollowerCount { get; } = new();

        public void Bind()
        {
            InitializeDatabase();

            Phone.Bind();
            Bag.Bind();
            Capture.Bind();
            Conversation.Bind();
        }

        public void Start()
        {
            LoadPlayerProperties();

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
        }

        public void Unbind()
        {
            Phone.Unbind();
            Bag.Unbind();
            Capture.Unbind();
            Conversation.Unbind();
        }

        private void InitializeDatabase()
        {
            if (_initialized) return;
            _initialized = true;
            // using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false })) ;
            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }

        private PlayerPropertiesV001 LoadPlayerProperties()
        {
            PlayerPropertiesV001 properties;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var playerPropertiesRepo = new PlayerPropertiesV001Repo(dbContext);
                properties = playerPropertiesRepo.Load();
                AddStamina(properties.Stamina);
            }

            return properties;
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
            SetMode(Mode == PlayerMode.Move ? PlayerMode.Phone : PlayerMode.Move);
        }

        public void SetBagMode()
        {
            ResetPlayerSubmenu();
            if (Mode != PlayerMode.Move && Mode != PlayerMode.OpenBag) return;
            SetMode(Mode == PlayerMode.Move ? PlayerMode.OpenBag : PlayerMode.Stop);
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
            SaveDbWriter.Add(dbContext =>
            {
                var repo = new PlayerPropertiesV001Repo(dbContext);
                var playerProperties = repo.Load();
                action(playerProperties);
                repo.Update(playerProperties);
            });
            LoadPlayerProperties();
        }

        public void SetMode(PlayerMode mode)
        {
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
        }

        public void AddStamina(int addition)
        {
            Stamina = Math.Min(AppConstants.MaxStamina, Stamina + addition);
            OnChangeStamina.OnNext(Stamina);
        }

        public void UpdateDayCount(int dayCount) => UpdateProperties(properties => properties.DayCount = dayCount);

        public int GetDayCount()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new PlayerPropertiesV001();
                return repo.DayCount;
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
            SaveDbWriter.Add(dbContext =>
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
    }
}
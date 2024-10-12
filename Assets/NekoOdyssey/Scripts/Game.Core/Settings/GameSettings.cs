using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Repo;
using NekoOdyssey.Scripts.Extensions;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Settings
{
    public class GameSettings
    {
        private GameSettingsV001 Settings { get; set; }

        public Locale Locale => Settings.Locale.ToEnum<Locale>();
        public float MasterVolume => Settings.MasterVolume;
        public float BgmVolume => Settings.BgmVolume;
        public float EffectVolume => Settings.EffectVolume;
        public int WindowMode => Settings.WindowMode;

        public Subject<Locale> OnChangeLocale { get; } = new();

        public void Bind()
        {
            InitializeSettings();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        private void InitializeSettings()
        {
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var repo = new GameSettingsV001Repo(dbContext);
                Settings = repo.Load();
                Debug.Log($">>locale<< db {Settings.Locale}");
            }
            OnChangeLocale.OnNext(Settings.Locale.ToEnum<Locale>());
        }

        public void SetLocale(Locale locale)
        {
            Settings.Locale = locale.ToText();
            OnChangeLocale.OnNext(locale);
            GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            {
                var repo = new GameSettingsV001Repo(dbContext);
                repo.Update(Settings);
            });
        }
    }
}
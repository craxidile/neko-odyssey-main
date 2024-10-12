using System;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Repo;
using Unity.VisualScripting;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis
{
    public class ClearSaveDatabase : MonoBehaviour
    {
        private void Start()
        {
            GameSettingsV001 settings;
            using (var context =
                   new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = true }))
            {
                var settingsRepo = new GameSettingsV001Repo(context);
                settings = settingsRepo.Load();
            }

            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;

            using (var context =
                   new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false }))
            {
                var settingsRepo = new GameSettingsV001Repo(context);
                settingsRepo.Update(settings);
            }
        }
    }
}
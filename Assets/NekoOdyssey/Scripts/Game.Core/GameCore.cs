using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Areas;
using NekoOdyssey.Scripts.Game.Core.Cat;
using NekoOdyssey.Scripts.Game.Core.Settings;
using NekoOdyssey.Scripts.Game.Core.Simulators;
using NekoOdyssey.Scripts.Game.Core.Uis;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCore
    {
        public GameSettings Settings { get; } = new();
        public Player.Player Player { get; } = new();
        public Metadata.Metadata Metadata { get; } = new();
        public MasterData.MasterData MasterData { get; } = new();
        public CatCollection Cats { get; } = new();
        public GameAis Ais { get; } = new();
        public GameUis Uis { get; } = new();
        public GameSimulators Simulators { get; } = new();
        public GameAreas Areas { get; } = new();
        public global::Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();

        public GameScene.GameScene GameScene { get; } = new();
        public SaveV001DbWriter SaveDbWriter { get; } = new();

        public Routine.Routine Routine { get; } = new(); // Linias Edit
        public EndDay.EndDayController EndDay { get; } = new(); // Linias Edit

        public void Bind()
        {
            InitializeSaveDatabase();

            Settings.Bind();
            Metadata.Bind();
            MasterData.Bind();
            Player.Bind();
            Ais.Bind();
            Uis.Bind();
            Simulators.Bind();
            Areas.Bind();
            PlayerMenu.Bind();
            PlayerMenuCandidateManager.Bind();
            GameScene.Bind();
            Routine.Bind();
            EndDay.Bind();
        }

        public void Start()
        {
            Settings.Start();
            Metadata.Start();
            MasterData.Start();
            Player.Start();
            Ais.Start();
            Uis.Start();
            Simulators.Start();
            Areas.Start();
            PlayerMenu.Start();
            PlayerMenuCandidateManager.Start();
            GameScene.Start();
            Routine.Start();
            EndDay.Start();
        }

        public void Unbind()
        {
            Settings.Unbind();
            Metadata.Unbind();
            MasterData.Unbind();
            Player.Unbind();
            Ais.Unbind();
            Uis.Unbind();
            Simulators.Unbind();
            Areas.Unbind();
            PlayerMenu.Unbind();
            PlayerMenuCandidateManager.Unbind();
            GameScene.Unbind();
            Routine.Unbind();
            EndDay.Unbind();
        }

        private void InitializeSaveDatabase()
        {
            // using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false })) ;
        }
    }
}

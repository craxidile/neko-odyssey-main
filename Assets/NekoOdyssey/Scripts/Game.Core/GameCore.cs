using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Areas;
using NekoOdyssey.Scripts.Game.Core.Audios;
using NekoOdyssey.Scripts.Game.Core.Cat;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Settings;
using NekoOdyssey.Scripts.Game.Core.Simulators;
using NekoOdyssey.Scripts.Game.Core.Uis;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using UniRx;
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
        public GameAudios Audios { get; } = new();
        public GameAis Ais { get; } = new();
        public GameUis Uis { get; } = new();
        public GameSimulators Simulators { get; } = new();
        public GameAreas Areas { get; } = new();
        public global::NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();
        public Routine.Routine Routine { get; } = new(); // Linias Edit
        public Routine.RoutineManger RoutineManger { get; } = new(); // Linias Edit
        public EndDay.EndDayController EndDay { get; } = new(); // Linias Edit
        public GameScene.GameScene GameScene { get; } = new();
        public SaveV001DbWriter SaveDbWriter { get; } = new();

        public bool SaveReady { get; private set; }
        public GameCoreMode Mode { get; private set; }

        public Subject<Unit> OnSaveReady { get; } = new();

        public void Bind(GameCoreMode mode)
        {
            Mode = mode;
            InitializeSaveDatabase();

            Audios.Bind();
            Settings.Bind();
            Metadata.Bind();
            MasterData.Bind();
            Uis.Bind();

            if (Mode == GameCoreMode.All)
            {
                Player.Bind();
                Ais.Bind();
                Simulators.Bind();
                Areas.Bind();
                PlayerMenu.Bind();
                PlayerMenuCandidateManager.Bind();
                GameScene.Bind();
                //Routine.Bind();
                RoutineManger.Bind();
                EndDay.Bind();
            }
        }

        public void Start()
        {
            Audios.Start();
            Settings.Start();
            Metadata.Start();
            MasterData.Start();
            Uis.Start();

            if (Mode == GameCoreMode.All)
            {
                Player.Start();
                Ais.Start();
                Simulators.Start();
                Areas.Start();
                PlayerMenu.Start();
                PlayerMenuCandidateManager.Start();
                GameScene.Start();
                //Routine.Start();
                RoutineManger.Start();
                EndDay.Start();

                GameRunner.Instance.PlayerInputHandler.OnResetSaveTriggerred
                    .Subscribe(ResetSave)
                    .AddTo(GameRunner.Instance);
                
                // Remove this later
                new TestDialog().Test();
            }
        }

        public void Unbind()
        {
            Audios.Unbind();
            Settings.Unbind();
            Metadata.Unbind();
            MasterData.Unbind();
            Uis.Unbind();

            if (Mode == GameCoreMode.All)
            {
                Player.Unbind();
                Ais.Unbind();
                Simulators.Unbind();
                Areas.Unbind();
                PlayerMenu.Unbind();
                PlayerMenuCandidateManager.Unbind();
                GameScene.Unbind();
                //Routine.Unbind();
                RoutineManger.Unbind();
                EndDay.Unbind();
            }
        }

        private void InitializeSaveDatabase()
        {
            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false })) ;
            SaveReady = true;
            OnSaveReady.OnNext(default);
        }

        private void ResetSave(Unit _)
        {
            // using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }
    }
}
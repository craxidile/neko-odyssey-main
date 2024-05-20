using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Areas;
using NekoOdyssey.Scripts.Game.Core.Cat;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCore
    {
        public Player.Player Player { get; } = new();
        public Metadata.Metadata Metadata { get; } = new();
        public MasterData.MasterData MasterData { get; } = new();
        public CatCollection Cats { get; } = new();
        public GameAis Ais { get; } = new();
        public GameAreas Areas { get; } = new();
        public Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();

        public GameScene.GameScene GameScene { get; } = new();

        public Routine.Routine Routine { get; } = new(); //**Linias Edit
        public EndDay.EndDayController EndDay { get; } = new(); //**Linias Edit

        public void Bind()
        {
            Metadata.Bind();
            MasterData.Bind();
            Player.Bind();
            Ais.Bind();
            Areas.Bind();
            PlayerMenu.Bind();
            PlayerMenuCandidateManager.Bind();
            GameScene.Bind();

            Routine.Bind();
            EndDay.Bind();
        }

        public void Start()
        {
            Metadata.Start();
            MasterData.Start();
            Player.Start();
            Ais.Start();
            Areas.Start();
            PlayerMenu.Start();
            PlayerMenuCandidateManager.Start();
            GameScene.Start();

            Routine.Start();
            EndDay.Start();
        }

        public void Unbind()
        {
            Metadata.Unbind();
            MasterData.Unbind();
            Player.Unbind();
            Ais.Unbind();
            Areas.Unbind();
            PlayerMenu.Unbind();
            PlayerMenuCandidateManager.Unbind();
            GameScene.Unbind();

            Routine.Unbind();
            EndDay.Unbind();
        }

    }
}
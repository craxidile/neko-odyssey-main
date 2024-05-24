using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Areas;
using NekoOdyssey.Scripts.Game.Core.Cat;
using NekoOdyssey.Scripts.Game.Core.Scene;
using NekoOdyssey.Scripts.Game.Core.Simulators;
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
        public GameSimulators Simulators { get; } = new();
        public GameAreas Areas { get; } = new();
        public Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();
        public GameScene GameScene { get; } = new();
        
        public void Bind()
        {
            Metadata.Bind();
            MasterData.Bind();
            Player.Bind();
            Ais.Bind();
            Simulators.Bind();
            Areas.Bind();
            PlayerMenu.Bind();
            PlayerMenuCandidateManager.Bind();
            GameScene.Bind();
        }

        public void Start()
        {
            Metadata.Start();
            MasterData.Start();
            Player.Start();
            Ais.Start();
            Simulators.Start();
            Areas.Start();
            PlayerMenu.Start();
            PlayerMenuCandidateManager.Start();
            GameScene.Start();
        }

        public void Unbind()
        {
            Metadata.Unbind();
            MasterData.Unbind();
            Player.Unbind();
            Ais.Unbind();
            Simulators.Unbind();
            Areas.Unbind();
            PlayerMenu.Unbind();
            PlayerMenuCandidateManager.Unbind();
            GameScene.Unbind();
        }
        
    }
}
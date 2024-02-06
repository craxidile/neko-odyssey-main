using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Scene;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCoreRunner
    {
        public Player.Player Player { get; } = new();
        public GameAis Ais { get; } = new();
        public Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();
        public GameScene GameScene { get; } = new();
        
        public void Bind()
        {
            Player.Bind();
            Ais.Bind();
            PlayerMenu.Bind();
            PlayerMenuCandidateManager.Bind();
            GameScene.Bind();
        }

        public void Start()
        {
            Player.Start();
            Ais.Start();
            PlayerMenu.Start();
            PlayerMenuCandidateManager.Start();
            GameScene.Start();
        }

        public void Unbind()
        {
            Player.Unbind();
            Ais.Unbind();
            PlayerMenu.Unbind();
            PlayerMenuCandidateManager.Unbind();
            GameScene.Unbind();
        }
        
    }
}
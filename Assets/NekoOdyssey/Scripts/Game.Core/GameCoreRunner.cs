using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Scene;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCoreRunner
    {
        public Player.Player Player { get; } = new();
        public Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();

        public GameScene GameScene { get; } = new();
        
        public void Bind()
        {
            Player.Bind();
            PlayerMenu.Bind();
            GameScene.Bind();
        }

        public void Start()
        {
            Player.Start();
            PlayerMenu.Start();
            GameScene.Start();
        }

        public void Unbind()
        {
            Player.Unbind();
            PlayerMenu.Unbind();
            GameScene.Unbind();
        }
        
    }
}
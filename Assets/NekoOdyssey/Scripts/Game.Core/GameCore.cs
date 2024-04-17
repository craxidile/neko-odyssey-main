using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Core.Ais;
using NekoOdyssey.Scripts.Game.Core.Areas;
using NekoOdyssey.Scripts.Game.Core.Cat;
using NekoOdyssey.Scripts.Game.Core.Scene;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCore
    {
        public Player.Player Player { get; } = new();
        public CatCollection Cats { get; } = new();
        public GameAis Ais { get; } = new();
        public GameAreas Areas { get; } = new();
        public Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu PlayerMenu { get; } = new();
        public PlayerMenuCandidateManager PlayerMenuCandidateManager { get; } = new();
        public GameScene GameScene { get; } = new();
        
        public void Bind()
        {
            Player.Bind();
            Ais.Bind();
            Areas.Bind();
            PlayerMenu.Bind();
            PlayerMenuCandidateManager.Bind();
            GameScene.Bind();
        }

        public void Start()
        {
            Player.Start();
            Ais.Start();
            Areas.Start();
            PlayerMenu.Start();
            PlayerMenuCandidateManager.Start();
            GameScene.Start();
        }

        public void Unbind()
        {
            Player.Unbind();
            Ais.Unbind();
            Areas.Unbind();
            PlayerMenu.Unbind();
            PlayerMenuCandidateManager.Unbind();
            GameScene.Unbind();
        }
        
    }
}
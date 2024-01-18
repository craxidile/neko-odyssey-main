using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class GameCoreRunner
    {
        public Player.Player Player { get; } = new();

        public void Bind()
        {
            Player.Bind();
        }

        public void Start()
        {
            Player.Start();
        }

        public void Unbind()
        {
            Player.Unbind();
        }
        
    }
}
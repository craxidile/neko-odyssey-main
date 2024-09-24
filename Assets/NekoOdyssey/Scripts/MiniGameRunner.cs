using System;
using NekoOdyssey.Scripts.MiniGame.Core;
using NekoOdyssey.Scripts.MiniGame.Unity.Connector;
using UnityEngine;

namespace NekoOdyssey.Scripts
{
    public class MiniGameRunner : MonoBehaviour
    {
        public static MiniGameRunner Instance { get; set; }

        public MiniGameCore Core { get; } = new();
        
        public MiniGameConnector Connector { get; set; }

        private void Awake()
        {
            Instance = this;
            Connector = FindFirstObjectByType<MiniGameConnector>();
            Core.Bind();
        }

        private void Start()
        {
            Core.Start();
        }

        private void OnDestroy()
        {
            Core.Unbind();
        }
    }
}
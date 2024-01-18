using System;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using Unity.VisualScripting;

namespace NekoOdyssey.Scripts.Game.Unity
{
    public class GameRunner : MonoBehaviour
    {
        public static GameRunner Instance;

        private PlayerInputActions _inputActions;

        public GameCoreRunner GameCore { get; } = new ();

        public PlayerInputHandler PlayerInputHandler { get; private set; }
        public UiInputHandler UiInputHandler { get; private set; }

        public GameRunner()
        {
            Instance = this;
        }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            PlayerInputHandler = gameObject.AddComponent<PlayerInputHandler>();
            PlayerInputHandler.InputActions = _inputActions;
            
            GameCore.Bind();
        }

        private void Start()
        {
            GameCore.Start();
        }

        private void OnDestroy()
        {
            GameCore.Unbind();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}
using System;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player
{
    public class Player
    {
        public PlayerMode Mode { get; private set; } = PlayerMode.Move;
        public bool Running { get; private set; } = false;
        public PlayerPhone Phone { get; private set; } = new();
            
        public GameObject GameObject { get; set; }
        
        public Subject<PlayerMode> OnChangeMode { get; } = new();
        public Subject<bool> OnRun { get; } = new();
        public Subject<Vector2> OnMove { get; } = new();

        private IDisposable _phoneTriggeredSubscription;
        private IDisposable _movingSubscription;
        private IDisposable _runningSubscription;

        public void Bind()
        {
            Phone.Bind();
        }

        public void Start()
        {
            _phoneTriggeredSubscription = GameRunner.Instance.PlayerInputHandler.OnPhoneTriggerred.Subscribe(_ =>
            {
                Mode = Mode == PlayerMode.Move ? PlayerMode.Phone : PlayerMode.Move;
                OnChangeMode.OnNext(Mode);
            });
            _movingSubscription = GameRunner.Instance.PlayerInputHandler.OnMove.Subscribe(input =>
            {
                if (Mode != PlayerMode.Move)
                {
                    OnMove.OnNext(new Vector2(0, 0));
                    return;
                }
                OnMove.OnNext(input);
            });
            _runningSubscription = GameRunner.Instance.PlayerInputHandler.OnRun.Subscribe(_ =>
            {
                Running = !Running;
                OnRun.OnNext(Running);
            });
            
            Phone.Start();
        }

        public void Unbind()
        {
            Phone.Unbind();
            _phoneTriggeredSubscription.Dispose();
            _movingSubscription.Dispose();
            _runningSubscription.Dispose();
        }
    }
}
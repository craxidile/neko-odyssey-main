using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Player.Capture;
using NekoOdyssey.Scripts.Game.Core.Player.Conversation;
using NekoOdyssey.Scripts.Game.Core.Player.Petting;
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
        public Vector3 Position { get; private set; }

        public PlayerPhone Phone { get; } = new();
        public PlayerCapture Capture { get; } = new();
        public PlayerPetting Petting { get; } = new();
        public PlayerConversation Conversation { get; } = new();

        public GameObject GameObject { get; set; }

        public Subject<PlayerMode> OnChangeMode { get; } = new();
        public Subject<bool> OnRun { get; } = new();
        public Subject<Vector2> OnMove { get; } = new();
        public Subject<Vector3> OnChangePosition { get; } = new();
 
        public void Bind()
        {
            Phone.Bind();
            Capture.Bind();
            Conversation.Bind();
        }

        public void SetMode(PlayerMode mode)
        {
            Mode = mode;
            OnChangeMode.OnNext(Mode);
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnPhoneTriggerred.Subscribe(_ =>
            {
                if (Mode != PlayerMode.Move && Mode != PlayerMode.Phone) return;
                Mode = Mode == PlayerMode.Move ? PlayerMode.Phone : PlayerMode.Move;
                OnChangeMode.OnNext(Mode);
            }).AddTo(GameRunner.Instance);
            
            GameRunner.Instance.PlayerInputHandler.OnMove.Subscribe(input =>
            {
                if (Mode != PlayerMode.Move)
                {
                    OnMove.OnNext(new Vector2(0, 0));
                    return;
                }
                OnMove.OnNext(input);
            }).AddTo(GameRunner.Instance);
            
            GameRunner.Instance.PlayerInputHandler.OnSpeedStart.Subscribe(_ =>
            {
                if (Running) return;
                OnRun.OnNext(Running = true);
            }).AddTo(GameRunner.Instance);
            
            GameRunner.Instance.PlayerInputHandler.OnSpeedEnd.Subscribe(_ =>
            {
                if (!Running) return;
                OnRun.OnNext(Running = false);
            }).AddTo(GameRunner.Instance);

            Phone.Start();
            Capture.Start();
            Conversation.Start();
        }

        public void Unbind()
        {
            Phone.Unbind();
            Capture.Unbind();
            Conversation.Unbind();
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            OnChangePosition.OnNext(position);
        }
    }
}
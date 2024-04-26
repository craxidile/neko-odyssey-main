using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Player.Bag;
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
        public PlayerBag Bag { get; } = new();
        public PlayerCapture Capture { get; } = new();
        public PlayerPetting Petting { get; } = new();
        public PlayerConversation Conversation { get; } = new();
        public PlayerStamina Stamina { get; set; }

        public GameObject GameObject { get; set; }

        public Subject<PlayerMode> OnChangeMode { get; } = new();
        public Subject<bool> OnRun { get; } = new();
        public Subject<Vector2> OnMove { get; } = new();
        public Subject<Vector3> OnChangePosition { get; } = new();

        public void Bind()
        {
            Phone.Bind();
            Bag.Bind();
            Capture.Bind();
            Conversation.Bind();

            //Stamina = GameObject.AddComponent<PlayerStamina>();
        }

        public void SetMode(PlayerMode mode)
        {
            Mode = mode;
            OnChangeMode.OnNext(Mode);
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnPhoneTriggerred
                .Subscribe(_ => SetPhoneMode())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnBagTriggerred
                .Subscribe(_ => SetBagMode())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnMove
                .Subscribe(HandleMove)
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnSpeedStart
                .Subscribe(_ => HandleSpeedStart())
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnSpeedEnd
                .Subscribe(_ => HandleSpeedEnd())
                .AddTo(GameRunner.Instance);

            Phone.Start();
            Bag.Start();
            Capture.Start();
            Conversation.Start();
        }

        public void Unbind()
        {
            Phone.Unbind();
            Bag.Unbind();
            Capture.Unbind();
            Conversation.Unbind();
        }

        private void ResetPlayerSubmenu()
        {
            if (Mode != PlayerMode.Submenu) return;
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(0);
        }

        public void SetPhoneMode()
        {
            ResetPlayerSubmenu();
            if (Mode != PlayerMode.Move && Mode != PlayerMode.Phone) return;
            Mode = Mode == PlayerMode.Move ? PlayerMode.Phone : PlayerMode.Move;
            OnChangeMode.OnNext(Mode);
        }

        public void SetBagMode()
        {
            ResetPlayerSubmenu();
            if (Mode != PlayerMode.Move && Mode != PlayerMode.OpenBag) return;
            Mode = Mode == PlayerMode.Move ? PlayerMode.OpenBag : PlayerMode.Stop;
            OnChangeMode.OnNext(Mode);
            // switch (Mode)
            // {
            //     case PlayerMode.Move:
            //         SetMode(PlayerMode.OpenBag);
            //         break;
            //     case PlayerMode.OpenBag:
            //         SetMode(PlayerMode.CloseBag);
            //         break;
            // }
        }

        private void HandleMove(Vector2 input)
        {
            if (Mode != PlayerMode.Move)
            {
                OnMove.OnNext(new Vector2(0, 0));
                return;
            }

            OnMove.OnNext(input);
        }

        private void HandleSpeedStart()
        {
            if (Running) return;
            OnRun.OnNext(Running = true);
        }

        private void HandleSpeedEnd()
        {
            if (!Running) return;
            OnRun.OnNext(Running = false);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            OnChangePosition.OnNext(position);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone
{
    public class PlayerPhone
    {
        private readonly List<PlayerPhoneApp> _phoneAppList = new()
        {
            PlayerPhoneApp.SocialNetwork,
            PlayerPhoneApp.PhotoGallery,
            PlayerPhoneApp.CatNote
        };

        private int _previousAppIndex;
        private int _currentAppIndex;
        private bool _scrolling;

        private static bool IsPhoneMode => GameRunner.Instance.Core.Player.Mode == PlayerMode.Phone;

        private static bool PhoneClosing =>
            !IsPhoneMode && GameRunner.Instance.Core.Player.PreviousMode == PlayerMode.Phone;

        public PlayerPhoneApp PreviousApp => _phoneAppList[_previousAppIndex];
        public PlayerPhoneApp CurrentApp => _phoneAppList[_currentAppIndex];

        public SocialNetworkApp SocialNetwork { get; } = new();
        public PhotoGalleryApp PhotoGallery { get; } = new();
        public CatNoteApp CatNote { get; } = new();

        public Subject<Vector2> OnScroll { get; } = new();
        public Subject<Unit> OnStopScrolling { get; } = new();
        public Subject<PlayerPhoneApp> OnChangeApp { get; } = new();

        public GameObject GameObject { get; set; }


        public void Bind()
        {
            SocialNetwork.Bind();
            PhotoGallery.Bind();
            CatNote.Bind();
        }

        public void Start()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerInputHandler = GameRunner.Instance.PlayerInputHandler;

            playerInputHandler.OnMove
                .Subscribe(HandleScroll)
                .AddTo(GameRunner.Instance);

            playerInputHandler.OnNextMenuTriggerred
                .Subscribe(HandleNextMenuInput)
                .AddTo(GameRunner.Instance);

            playerInputHandler.OnPrevMenuTriggerred
                .Subscribe(HandlePreviousMenuInput)
                .AddTo(GameRunner.Instance);

            player.OnChangeMode
                .Subscribe(ResetCurrentIndex)
                .AddTo(GameRunner.Instance);

            SocialNetwork.Start();
            PhotoGallery.Start();
            CatNote.Start();
        }

        public void Unbind()
        {
            SocialNetwork.Unbind();
            PhotoGallery.Unbind();
            CatNote.Unbind();
        }

        private void HandleScroll(Vector2 input)
        {
            if (!IsPhoneMode)
            {
                _scrolling = false;
                return;
            }

            if (input.y == 0)
            {
                if (_scrolling) OnStopScrolling.OnNext(default);
                _scrolling = false;
                return;
            }

            _scrolling = true;
            OnScroll.OnNext(input);
        }

        private void HandleNextMenuInput(Unit _)
        {
            if (!IsPhoneMode) return;
            var index = _currentAppIndex;
            var nextIndex = Math.Min(_phoneAppList.Count - 1, index + 1);
            if (nextIndex == index) return;
            SetAppIndices(index, nextIndex);
        }

        private void HandlePreviousMenuInput(Unit _)
        {
            if (!IsPhoneMode) return;
            var index = _currentAppIndex;
            var prevIndex = Math.Max(0, index - 1);
            if (prevIndex == index) return;
            SetAppIndices(index, prevIndex);
        }

        private void ResetCurrentIndex(PlayerMode mode)
        {
            if (!PhoneClosing) return;
            DOVirtual.DelayedCall(.3f, () =>
            {
                SetAppIndices(_currentAppIndex, 0);
            });
        }

        private void SetAppIndices(int previousIndex, int nextIndex)
        {
            _previousAppIndex = previousIndex;
            _currentAppIndex = nextIndex;
            OnChangeApp.OnNext(CurrentApp);
        }
    }
}
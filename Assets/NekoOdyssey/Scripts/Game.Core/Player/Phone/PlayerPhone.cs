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
        
        private static int running = 1;
        
        private readonly List<PlayerPhoneApp> _phoneAppList = new()
        {
            PlayerPhoneApp.SocialNetwork,
            PlayerPhoneApp.PhotoGallery,
        };

        private int _previousAppIndex;
        private int _currentAppIndex;

        public PlayerPhoneApp PreviousApp => _phoneAppList[_previousAppIndex];
        public PlayerPhoneApp CurrentApp => _phoneAppList[_currentAppIndex];

        public SocialNetworkApp SocialNetwork { get; } = new();
        public PhotoGalleryApp PhotoGallery { get; } = new();

        public readonly Subject<Vector2> OnScroll = new();
        public readonly Subject<PlayerPhoneApp> OnChangeApp = new();

        private int _id;

        public GameObject GameObject { get; set; }

        public PlayerPhone()
        {
            _id = running++;
        }

        public void Bind()
        {
            SocialNetwork.Bind();
            PhotoGallery.Bind();
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnMove.Subscribe(input =>
            {
                if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Phone || input.y == 0) return;
                OnScroll.OnNext(input);
            }).AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnNextMenuTriggerred.Subscribe(_ =>
            {
                if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Phone) return;
                var index = _currentAppIndex;
                var nextIndex = Math.Min(_phoneAppList.Count - 1, index + 1);
                if (nextIndex == index) return;
                Debug.Log($">>canvas_swap<< check 02 *{_id}* {index} {nextIndex}");
                SetAppIndices(index, nextIndex);
            }).AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred.Subscribe(_ =>
            {
                if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Phone) return;
                var index = _currentAppIndex;
                var prevIndex = Math.Max(0, index - 1);
                if (prevIndex == index) return;
                Debug.Log($">>canvas_swap<< check 02 *{_id}* {index} {prevIndex}");
                SetAppIndices(index, prevIndex);
            }).AddTo(GameRunner.Instance);

            GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(mode =>
            {
                if (mode != PlayerMode.Phone) return;
                ResetCurrentIndex();
            }).AddTo(GameRunner.Instance);

            SocialNetwork.Start();
            PhotoGallery.Start();
        }

        public void Unbind()
        {
            SocialNetwork.Unbind();
            PhotoGallery.Unbind();
        }

        private void ResetCurrentIndex()
        {
            Debug.Log($">>canvas_swap<< check 03 *{_id}* {_previousAppIndex} {0}");
            SetAppIndices(_currentAppIndex, 0);
        }

        private void SetAppIndices(int previousIndex, int nextIndex)
        {
            Debug.Log($">>canvas_swap<< check 04 *{_id}* {previousIndex} {nextIndex}");
            _previousAppIndex = previousIndex;
            _currentAppIndex = nextIndex;
            OnChangeApp.OnNext(CurrentApp);
        }
    }
}
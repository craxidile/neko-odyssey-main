using System;
using System.Collections.Generic;
using System.Reflection;
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
        };

        private int _previousAppIndex = 0;
        private int _currentAppIndex = 0;

        public PlayerPhoneApp PreviousApp => _phoneAppList[_previousAppIndex];
        public PlayerPhoneApp CurrentApp => _phoneAppList[_currentAppIndex];

        public SocialNetworkApp SocialNetwork { get; } = new();
        public PhotoGalleryApp PhotoGallery { get; } = new();

        public readonly Subject<Vector2> OnScroll = new();
        public readonly Subject<PlayerPhoneApp> OnChangeApp = new();

        public GameObject GameObject { get; set; }

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
                _previousAppIndex = index;
                _currentAppIndex = nextIndex;
                OnChangeApp.OnNext(CurrentApp);
            }).AddTo(GameRunner.Instance);
            
            GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred.Subscribe(_ =>
            {
                if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Phone) return;
                var index = _currentAppIndex;
                var prevIndex = Math.Max(0, index - 1);
                if (prevIndex == index) return;
                _previousAppIndex = index;
                _currentAppIndex = prevIndex;
                OnChangeApp.OnNext(CurrentApp);
            }).AddTo(GameRunner.Instance);
            SocialNetwork.Start();
            PhotoGallery.Start();
        }

        public void Unbind()
        {
            SocialNetwork.Unbind();
            PhotoGallery.Unbind();
        }
    }
}
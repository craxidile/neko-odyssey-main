using System;
using System.Collections.Generic;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Feed;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Feed
{
    public class CentralPlayerFeedHandler : MonoBehaviour
    {
        private static readonly HashSet<PlayerMenuAction> EligibleActions = new()
        {
            PlayerMenuAction.FeedFish000,
            PlayerMenuAction.FeedFish001,
            PlayerMenuAction.FeedFish002,
            PlayerMenuAction.FeedFish003,
            PlayerMenuAction.FeedFish004,
            PlayerMenuAction.FeedFish005,
            PlayerMenuAction.FeedFish006,
            PlayerMenuAction.FeedFish007,
            PlayerMenuAction.FeedFish008,
            PlayerMenuAction.FeedFish009,
            PlayerMenuAction.FeedFish010,
            PlayerMenuAction.FeedFish011,
            PlayerMenuAction.FeedCan,
        };
        
        private Core.Player.Player _player;
        private Core.PlayerMenu.PlayerMenu _playerMenu;
        private PlayerFeed _feed;

        private void Awake()
        {
            _player = GameRunner.Instance.Core.Player;
            _playerMenu = GameRunner.Instance.Core.PlayerMenu;
            _feed = GameRunner.Instance.Core.Player.Feed;
        }

        private void Start()
        {
            _playerMenu.OnCommitAction
                .Subscribe(HandlePlayerMenuAction)
                .AddTo(this);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            Debug.Log($">>feed<< action {action} {EligibleActions.Contains(action)}");
            if (!EligibleActions.Contains(action)) return;
            _playerMenu.SetActive(false);
            HandleFeedAction(action);
        }

        private void HandleFeedAction(PlayerMenuAction action)
        {
            var menuGameObject = _playerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<FeedAttributes>();
            if (attributes == null) return;

            _feed.Mode = attributes.feedMode;
            _feed.TargetPosition = attributes.feedAnchor.position;
            if (!_feed.FeedCat(action)) return;
            
            GameRunner.Instance.Core.Cats.CurrentCatCode = attributes.catCode;
            GameRunner.Instance.Core.Cats.CurrentCat?.Eat(true);
            
            _player.SetMode(PlayerMode.Feed);
            Debug.Log($">>feed<< mode {_player.Mode}");
        }
    }
}
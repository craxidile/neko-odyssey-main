using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Feed;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Feed
{
    public class PlayerFeed
    {
        private static readonly Dictionary<PlayerMenuAction, string> ActionToCodeMap = new()
        {
            { PlayerMenuAction.FeedFish000, "CatFoodFish000" },
            { PlayerMenuAction.FeedFish001, "CatFoodFish001" },
            { PlayerMenuAction.FeedFish002, "CatFoodFish002" },
            { PlayerMenuAction.FeedFish003, "CatFoodFish003" },
            { PlayerMenuAction.FeedFish004, "CatFoodFish004" },
            { PlayerMenuAction.FeedFish005, "CatFoodFish005" },
            { PlayerMenuAction.FeedFish006, "CatFoodFish006" },
            { PlayerMenuAction.FeedFish007, "CatFoodFish007" },
            { PlayerMenuAction.FeedFish008, "CatFoodFish008" },
            { PlayerMenuAction.FeedFish009, "CatFoodFish009" },
            { PlayerMenuAction.FeedFish010, "CatFoodFish010" },
            { PlayerMenuAction.FeedFish011, "CatFoodFish011" },
            { PlayerMenuAction.FeedCan, "CatFoodCan001" },
        };

        public Vector3 TargetPosition { get; set; }
        public FeedMode Mode { get; set; }

        public Subject<Unit> OnFinishFeed { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public bool FeedCat(PlayerMenuAction action)
        {
            if (!ActionToCodeMap.TryGetValue(action, out var itemCode))
            {
                GameRunner.Instance.Core.Player.ShakeHead();
                return false;
            }

            if (!GameRunner.Instance.Core.Player.Bag.UseBagItem(itemCode))
            {
                GameRunner.Instance.Core.Player.ShakeHead();
                return false;
            }

            return true;
        }

        public void Finish()
        {
            OnFinishFeed.OnNext(Unit.Default);

            GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteNameActive();
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(0);
            Debug.Log("<color=red>>>player_mode<< move</color>");
        }
    }
}
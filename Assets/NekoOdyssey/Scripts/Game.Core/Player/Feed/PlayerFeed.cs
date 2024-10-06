using NekoOdyssey.Scripts.Game.Core.Feed;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Feed
{
    public class PlayerFeed
    {
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

        public void FeedCat(PlayerMenuAction action)
        {
            string itemName = null;
            switch (action)
            {
                case PlayerMenuAction.FeedFish000:
                    itemName = "CatFoodFish001";
                    break;
                case PlayerMenuAction.FeedFish001:
                    itemName = "CatFoodFish002";
                    break;
                case PlayerMenuAction.FeedFish002:
                    itemName = "CatFoodFish003";
                    break;
                case PlayerMenuAction.FeedFish003:
                    itemName = "CatFoodFish004";
                    break;
                case PlayerMenuAction.FeedFish004:
                    itemName = "CatFoodFish005";
                    break;
                case PlayerMenuAction.FeedFish005:
                    itemName = "CatFoodFish006";
                    break;
                case PlayerMenuAction.FeedFish006:
                    itemName = "CatFoodFish007";
                    break;
                case PlayerMenuAction.FeedFish007:
                    itemName = "CatFoodFish008";
                    break;
                case PlayerMenuAction.FeedFish008:
                    itemName = "CatFoodFish009";
                    break;
                case PlayerMenuAction.FeedFish009:
                    itemName = "CatFoodFish010";
                    break;
                case PlayerMenuAction.FeedFish010:
                    itemName = "CatFoodFish011";
                    break;
                default: return;
            }
            GameRunner.Instance.Core.Player.Bag.UseBagItem(itemName);
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
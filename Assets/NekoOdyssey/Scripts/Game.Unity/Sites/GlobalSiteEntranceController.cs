using System;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Sites
{
    public class GlobalSiteEntranceController : MonoBehaviour
    {
        private void Start()
        {
            GameRunner.Instance.GameCore.PlayerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            if (action == PlayerMenuAction.Exit)
            {
                return;
            }

            if (action != PlayerMenuAction.Enter) return;
            GameRunner.Instance.GameCore.GameScene.CloseScene();

            string sceneName = null;
            switch (GameRunner.Instance.GameCore.PlayerMenu.Site)
            {
                case PlayerMenuSite.UdonNekoInside11:
                    sceneName = $"NekoInside11Udon";
                    break;
                case PlayerMenuSite.HouseNekoInside18:
                    sceneName = $"NekoInside18House";
                    break;
                case PlayerMenuSite.GroceryStoreInside12:
                    sceneName = $"NekoInside12Shop";
                    break;
                case PlayerMenuSite.FlagShopInside19:
                    sceneName = $"NekoInside19FlagShop";
                    break;
                case PlayerMenuSite.BookStoreInside07:
                    sceneName = $"NekoInside07BookStore";
                    break;
                case PlayerMenuSite.RamenShopInside01:
                    sceneName = $"NekoInside01";
                    break;
            }

            if (sceneName == null) return;
            
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
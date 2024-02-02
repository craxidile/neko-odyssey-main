using System;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Sites
{
    public class GlobalSiteEntranceController : MonoBehaviour
    {
        private void Start()
        {
            GameRunner.Instance.Core.PlayerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            
            if (action != PlayerMenuAction.Enter && action != PlayerMenuAction.Exit) return;
            
            GameRunner.Instance.Core.GameScene.CloseScene();
            
            if (action == PlayerMenuAction.Exit)
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    SceneManager.LoadScene($"Neko2", LoadSceneMode.Single);
                    SceneManager.LoadScene($"Neko08", LoadSceneMode.Additive);
                    SceneManager.LoadScene($"CatScene", LoadSceneMode.Additive);
                    SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
                    SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
                    SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
                });
                return;
            }

            string sceneName = null;
            switch (GameRunner.Instance.Core.PlayerMenu.Site)
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

            DOVirtual.DelayedCall(2, () =>
            {
                PlayerController.MainPlayerAnchor = GameRunner.Instance.Core.Player.GameObject.transform.position;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
            });
        }
    }
}
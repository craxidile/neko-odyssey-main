using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Sites
{
    public class CentralSiteDoorController : MonoBehaviour
    {
        private const float EntryDelay = 1.5f;
        private const float ExitDelay = 2f;
        private readonly Dictionary<PlayerMenuSite, string> _siteSceneMap = new()
        {
            { PlayerMenuSite.UdonNekoInside11, $"NekoInside11Udon" },
            { PlayerMenuSite.HouseNekoInside18, $"NekoInside18House" },
            { PlayerMenuSite.GroceryStoreInside12, $"NekoInside12Shop" },
            { PlayerMenuSite.FlagShopInside19, $"NekoInside19FlagShop" },
            { PlayerMenuSite.BookStoreInside07, $"NekoInside07BookStore" },
            { PlayerMenuSite.RamenShopInside01, $"NekoInside01" },
        };
            
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
                DOVirtual.DelayedCall(EntryDelay, () =>
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

            var site = GameRunner.Instance.Core.PlayerMenu.Site;
            if (!_siteSceneMap.ContainsKey(site)) return;
            
            var sceneName = _siteSceneMap[site];
            if (sceneName == null) return;

            DOVirtual.DelayedCall(ExitDelay, () =>
            {
                var player = GameRunner.Instance.Core.Player;
                PlayerController.MainPlayerAnchor = player.GameObject.transform.position;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
            });
        }
    }
}
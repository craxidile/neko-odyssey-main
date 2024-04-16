using System;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Sites
{
    public class CentralSiteActionController : MonoBehaviour
    {
        private const float EntryDelay = 1.5f;
        private const float ExitDelay = 2f;
        
        private void Start()
        {
            GameRunner.Instance.Core.PlayerMenu.OnCommitAction
                .Subscribe(HandlePlayerMenuAction)
                .AddTo(this);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            Debug.Log($">>commit_player_menu<< {action}");
            if (
                action != PlayerMenuAction.Left &&
                action != PlayerMenuAction.Right
            ) return;
            
            GameRunner.Instance.Core.GameScene.CloseScene();
            DOVirtual.DelayedCall(ExitDelay, () =>
            {
                Debug.Log($">>move_to_site<< {GameRunner.Instance.Core.PlayerMenu.SiteName}");
                SiteRunner.Instance.Core.Site.SetSite(GameRunner.Instance.Core.PlayerMenu.SiteName);
            });
        }
    }
    
}
using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Sites
{
    public class CentralSiteActionController : MonoBehaviour
    {
        private const float EntryDelay = 1.5f;
        private const float ExitDelay = 2f;

        private static Vector3? _previousPosition;

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
                action != PlayerMenuAction.Enter &&
                action != PlayerMenuAction.Exit &&
                action != PlayerMenuAction.Left &&
                action != PlayerMenuAction.Right &&
                action != PlayerMenuAction.LeftEnter &&
                action != PlayerMenuAction.RightEnter &&
                action != PlayerMenuAction.LeftExit &&
                action != PlayerMenuAction.RightExit
            ) return;

            if (
                action != PlayerMenuAction.Exit &&
                action != PlayerMenuAction.LeftExit &&
                action != PlayerMenuAction.RightExit
            )
            {
                _previousPosition = GameRunner.Instance.Core.Player.Position;
            }

            GameRunner.Instance.Core.GameScene.CloseScene();
            DOVirtual.DelayedCall(ExitDelay, () =>
            {
                if (
                    action != PlayerMenuAction.Exit &&
                    action != PlayerMenuAction.LeftExit &&
                    action != PlayerMenuAction.RightExit
                )
                    SiteRunner.Instance.Core.Site.SetSite(GameRunner.Instance.Core.PlayerMenu.SiteName);
                else
                {
                    SiteRunner.Instance.Core.Site.MoveToPreviousSite(_previousPosition);
                    _previousPosition = null;
                }
            });
        }
    }
}
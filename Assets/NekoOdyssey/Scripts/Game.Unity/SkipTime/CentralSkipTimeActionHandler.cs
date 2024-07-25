using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.SkipTime
{
    public class CentralSkipTimeActionHandler : MonoBehaviour
    {
        private void Start()
        {
            var playerMenu = GameRunner.Instance.Core.PlayerMenu;
            playerMenu.OnCommitAction
                .Subscribe(HandlePlayerMenuAction)
                .AddTo(this);
        }

        private static void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            var player = GameRunner.Instance.Core.Player;
            var playerMenu = GameRunner.Instance.Core.PlayerMenu;

            if (action != PlayerMenuAction.SkipTime) return;
            playerMenu.SetActive(false);
            //SoundEffectController.Instance.talk.Play();

            //var menuGameObject = playerMenu.GameObject;
            //var attributes = menuGameObject.GetComponent<SkipTimeAttributes>();
            //if (attributes == null) return;

            //player.Conversation.Dialog = attributes.dialog;
            //player.SetMode(PlayerMode.Conversation);

            GameRunner.Instance.TimeRoutine.PauseTime();

            player.SetMode(PlayerMode.EndDay_TimeOut);


            Debug.Log("HandlePlayerMenuAction SkipTime");
        }
    }

}
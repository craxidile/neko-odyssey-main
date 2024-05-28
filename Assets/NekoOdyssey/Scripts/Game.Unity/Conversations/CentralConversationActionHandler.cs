using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.Conversations
{
    public class CentralConversationActionHandler : MonoBehaviour
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
            
            if (action != PlayerMenuAction.Information) return;
            playerMenu.SetActive(false);
            SoundEffectController.Instance.talk.Play();

            var menuGameObject = playerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<ConversationAttributes>();
            if (attributes == null) return;

            player.Conversation.Dialog = attributes.dialog;
            player.SetMode(PlayerMode.Conversation);


            Debug.Log("HandlePlayerMenuAction");
        }
    }
}
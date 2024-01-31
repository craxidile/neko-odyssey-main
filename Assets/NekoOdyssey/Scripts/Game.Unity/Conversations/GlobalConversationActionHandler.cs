using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.Conversations
{
    public class GlobalConversationActionHandler : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(">>handle<< hh");
            GameRunner.Instance.GameCore.PlayerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            Debug.Log($">>handle<< {action}");
            if (action != PlayerMenuAction.Information) return;
            GameRunner.Instance.GameCore.PlayerMenu.SetActive(false);
            SoundEffectController.Instance.talk.Play();

            var menuGameObject = GameRunner.Instance.GameCore.PlayerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<ConversationAttributes>();
            if (attributes == null) return;

            GameRunner.Instance.GameCore.Player.Conversation.Dialog = attributes.dialog;

            Debug.Log($">>handle<< eee");
            GameRunner.Instance.GameCore.Player.SetMode(PlayerMode.Conversation);
        }
    }
}
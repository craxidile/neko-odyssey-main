using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Capture;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Conversation
{
    public class PlayerConversation
    {
        public string Dialog { get; set; }

        public void Bind()
        {
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnFireTriggerred
                .Subscribe(HandleConversation)
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }

        private void HandleConversation(Unit _)
        {
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Conversation) return;
            DOVirtual.DelayedCall(.5f, () =>
            {
                GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
                Debug.Log($">>set_mode_move<< 02");
            });
        }
    }
}
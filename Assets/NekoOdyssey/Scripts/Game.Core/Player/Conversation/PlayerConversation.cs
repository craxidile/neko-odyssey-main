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

        private IDisposable _fireTriggeredSubscription;

        public void Bind()
        {
        }

        public void Start()
        {
            Debug.Log($">>set_mode_move<< 00");
            _fireTriggeredSubscription = GameRunner.Instance.PlayerInputHandler.OnFireTriggerred.Subscribe(_ =>
            {
                Debug.Log($">>set_mode_move<< 01 {GameRunner.Instance.GameCore.Player.Mode}");
                if (GameRunner.Instance.GameCore.Player.Mode != PlayerMode.Conversation) return;
                DOVirtual.DelayedCall(.5f, () =>
                {
                    GameRunner.Instance.GameCore.Player.SetMode(PlayerMode.Move);
                    Debug.Log($">>set_mode_move<< 02");
                });
            });
        }

        public void Unbind()
        {
            _fireTriggeredSubscription.Dispose();
        }
    }
}
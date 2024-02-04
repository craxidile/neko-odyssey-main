using System;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Conversations
{
    public class PlayerConversationController : MonoBehaviour
    {
        private bool _active;

        // Game Objects
        private Animator _animator;
        private SpriteRenderer _renderer;
        private GameObject _dialogCanvas;
        private PlayerMode _previousMode;

        private IDisposable _playerModeChangedSubscription;
        private IDisposable _activeChangeSubscription;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Conversation;
            if (!_active) return;

            if (!GameRunner.Instance.AssetMap.ContainsKey("dialogcanvas")) return;
            
            _dialogCanvas = Instantiate(
                GameRunner.Instance.AssetMap["dialogcanvas"],
                GameRunner.Instance.Core.PlayerMenu.GameObject.transform
            ) as GameObject;
            Debug.Log($">>dialog_canvas<< {_dialogCanvas}");
            var dialogCanvasController = _dialogCanvas.GetComponent<DialogCanvasController>();
            dialogCanvasController.messageBox.text = GameRunner.Instance.Core.Player.Conversation.Dialog;
        }


        public void Awake()
        {
            var playerController = GameRunner.Instance.Core.Player.GameObject.GetComponent<PlayerController>();
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            _playerModeChangedSubscription = GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive);
            GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(mode =>
            {
                if (mode != PlayerMode.Conversation && _dialogCanvas != null)
                    Destroy(_dialogCanvas);

            });
        }

        public void OnDestroy()
        {
            _playerModeChangedSubscription.Dispose();
        }
    }
}
using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Capture
{
    public class CentralCaptureActionHandler : MonoBehaviour
    {
        private Core.Player.Player _player;
        private Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu _playerMenu;
        private Core.Player.Capture.PlayerCapture _capture;

        private void Awake()
        {
            _player = GameRunner.Instance.Core.Player;
            _playerMenu = GameRunner.Instance.Core.PlayerMenu;
            _capture = GameRunner.Instance.Core.Player.Capture;
        }

        private void Start()
        {
            _playerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            if (action != PlayerMenuAction.Camera) return;
            _playerMenu.SetActive(false);

            // Delay sound effect to
            DOVirtual.DelayedCall(2f, () => SoundEffectController.Instance.shutter.Play());
            
            var menuGameObject = _playerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<CaptureAttributes>();
            if (attributes == null) return;

            _capture.TargetPosition = attributes.captureAnchor.transform.position;
            _capture.Mode = attributes.captureMode;
            _capture.CatCode = attributes.catCode;

            _player.SetMode(PlayerMode.Capture);
        }
    }
}
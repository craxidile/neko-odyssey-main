using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Cameras
{
    public class ZoomCameraController : MonoBehaviour
    {
        private GameObject _zoomAnchor;

        private readonly List<PlayerMode> _modesToShowPlayerCamera = new()
        {
            PlayerMode.Phone,
            PlayerMode.OpenBag,
            PlayerMode.CloseBag
        };

        private bool _active;

        private void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _zoomAnchor = playerController.zoomAnchor;
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
        }

        private void Update()
        {
            if (!_active) return;
            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            var playerCamera = GameRunner.Instance.cameras.playerCamera;
            if (!playerCamera || !playerCamera.gameObject.activeSelf) return;
            playerCamera.fieldOfView = mainCamera.fieldOfView;
        }

        private void SetActive(PlayerMode mode)
        {
            _active = _modesToShowPlayerCamera.Contains(mode);

            _zoomAnchor.SetActive(_active);

            var playerCamera = GameRunner.Instance.cameras.playerCamera;
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(_active);
        }
    }
}
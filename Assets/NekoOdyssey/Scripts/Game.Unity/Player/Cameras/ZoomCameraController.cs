using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Cameras
{
    public class ZoomCameraController : MonoBehaviour
    {
        private readonly List<PlayerMode> _modesToShowPlayerCamera = new()
        {
            PlayerMode.Phone,
            PlayerMode.OpenBag,
            PlayerMode.CloseBag
        };

        private bool _active;

        private void Awake()
        {
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
            var playerCamera = GameRunner.Instance.cameras.playerCamera;
            
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            if (!_active)
            {
                playerController.phoneCameraAnchor.SetActive(false);
                playerController.bagCameraAnchor.SetActive(false);
            }
            else
            {
                var zoomAnchor = mode == PlayerMode.Phone
                    ? playerController.phoneCameraAnchor
                    : playerController.bagCameraAnchor;
                zoomAnchor.SetActive(_active);
            }

            if (playerCamera != null)
                playerCamera.gameObject.SetActive(_active);
        }
    }
}
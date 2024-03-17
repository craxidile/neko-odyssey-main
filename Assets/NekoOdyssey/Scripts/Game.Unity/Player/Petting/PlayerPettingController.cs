using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Capture;
using NekoOdyssey.Scripts.Game.Core.Petting;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Models;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Petting
{
    public class PlayerPettingController : MonoBehaviour
    {
        private bool _active;

        private GameObject _catPhoto;
        private Animator _animator;
        private SpriteRenderer _renderer;

        public void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
        }


        private void SetActive(PlayerMode mode)
        {
            Debug.Log($">>pet_mode<< {mode}");
            _active = mode == PlayerMode.Pet;
            if (!_active) return;
            HandlePetting();
        }


        private void HandlePetting()
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Petting"), 1f);
            
            Debug.Log($">>pet_mode<< {GameRunner.Instance.Core.Player.Petting.Mode}");
            string petMode;
            switch (GameRunner.Instance.Core.Player.Petting.Mode)
            {
                case PettingMode.Sit:
                    petMode = $"PetSit";
                    break;
                case PettingMode.Low:
                    petMode = $"PetLow";
                    break;
                case PettingMode.Medium:
                    petMode = $"PetMedium";
                    break;
                case PettingMode.High:
                    petMode = $"PetHigh";
                    break;
                case PettingMode.None:
                default:
                    return;
            }

            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            if (!mainCamera) return;
            var cameraTransform = mainCamera.transform;
            var forward = cameraTransform.forward;
            var left = -cameraTransform.right;

            var playerPosition = GameRunner.Instance.Core.Player.Position;
            var capturePosition = GameRunner.Instance.Core.Player.Petting.TargetPosition;
            var delta = capturePosition - playerPosition;
            var deltaDepth = forward.x != 0f ? forward.x * delta.x : forward.z * delta.z;
            var deltaSide = forward.x != 0f ? left.z * delta.z : left.x * delta.x;
            Debug.Log($">>forward<< {forward} {left} {deltaDepth} {deltaSide}");
            var angle = Mathf.Rad2Deg * Mathf.Atan2(Mathf.Abs(deltaSide), Mathf.Abs(deltaDepth));

            _animator.SetBool(petMode, true);
            _animator.SetFloat($"FacingToCat", deltaDepth <= 0f ? 0.0f : 1.0f);
            _animator.SetFloat($"CaptureAngle", angle);
            _renderer.flipX = deltaSide > 0f;

            DOVirtual.DelayedCall(2f, () =>
            {
                _animator.SetBool(petMode, false);
                _animator.SetLayerWeight(_animator.GetLayerIndex($"Petting"), 0);
                GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
            });
        }
    }
}
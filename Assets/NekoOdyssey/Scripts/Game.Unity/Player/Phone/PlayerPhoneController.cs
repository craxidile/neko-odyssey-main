using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Phone
{
    public class PlayerPhoneController : MonoBehaviour
    {
        private bool _previousActive;
        private bool _active;

        private GameObject _phoneScreen;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private DepthOfField _depthOfField;

        private void SetActive(PlayerMode mode)
        {
            _previousActive = _active;
            _active = mode == PlayerMode.Phone;

            _phoneScreen.SetActive(_active);
            var playerCamera = GameRunner.Instance.cameras.playerCamera;
            playerCamera.gameObject.SetActive(_active);

            if (_active) _animator.SetLayerWeight(_animator.GetLayerIndex($"Phone"), 1f);

            switch (_previousActive)
            {
                case false when _active:
                    AnimateOpening();
                    break;
                case true when !_active:
                    AnimateClosing();
                    break;
            }
        }

        private void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _phoneScreen = playerController.phoneScreen;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
        }

        private void Update()
        {
            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            var playerCamera = GameRunner.Instance.cameras.playerCamera;
            if (!playerCamera || !playerCamera.gameObject.activeSelf) return;
            playerCamera.fieldOfView = mainCamera.fieldOfView;
        }

        private void AnimateOpening()
        {
            _renderer.flipX = false;
            SoundEffectController.Instance.openPhone.Play();
        }

        private void AnimateClosing()
        {
            SoundEffectController.Instance.closePhone.Play();
        }
    }
}
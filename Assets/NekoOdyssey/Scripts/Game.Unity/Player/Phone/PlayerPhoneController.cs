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

        private Animator _animator;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
        }

        private void SetActive(PlayerMode mode)
        {
            _previousActive = _active;
            _active = mode == PlayerMode.Phone;

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
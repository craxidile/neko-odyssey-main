using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;

namespace NekoOdyssey.Scripts.Game.Unity.Player.EndDay
{
    public class PlayerEndDayController : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _renderer;

        bool _isExcecuted = false;

        public void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            GameRunner.Instance.Core.Player.Stamina.OnChangeStamina
                .Subscribe(CheckEndDayStaminaOut)
                .AddTo(this);
            GameRunner.Instance.TimeRoutine.OnTimeUpdate
                .Subscribe(_ => CheckEndDayTimeOut())
                .AddTo(this);

            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(CheckPlayerMode)
                .AddTo(this);
            GameRunner.Instance.Core.EndDay.OnStaminaOutFinish
                .Subscribe(HandleEndDayFinish_StaminaOut)
                .AddTo(this);
            GameRunner.Instance.Core.EndDay.OnTimeOutFinish
               .Subscribe(HandleEndDayFinish_TimeOut)
               .AddTo(this);
        }

        void CheckEndDayStaminaOut(int stamina)
        {
            if (_isExcecuted) return;
            if (stamina > 0) return;

            Debug.Log($"player EndDayStaminaOut");

            GameRunner.Instance.TimeRoutine.PauseTime();
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.EndDay_StaminaOut);

            _isExcecuted = true;
        }
        void CheckEndDayTimeOut()
        {
            if (_isExcecuted) return;

            var currentTime = GameRunner.Instance.TimeRoutine.currentTime;
            if (currentTime >= new TimeHrMin(AppConstants.Time.EndDayTime))
            {
                Debug.Log($"player EndDayTimeOut");

                GameRunner.Instance.TimeRoutine.PauseTime();
                GameRunner.Instance.Core.Player.SetMode(PlayerMode.EndDay_TimeOut);

                _isExcecuted = true;
            }

        }

        void CheckPlayerMode(PlayerMode mode)
        {
            Debug.Log($">>EndDay_mode<< {mode}");
            if (mode == PlayerMode.EndDay_StaminaOut)
            {
                HandleEndDay_StaminaOut();
            }
            if (mode == PlayerMode.EndDay_TimeOut)
            {
                HandleEndDay_TimeOut();
            }
        }


        void HandleEndDay_StaminaOut()
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex($"EndDay"), 1f);
            _animator.SetInteger("EndDayState", 1);
            // SoundEffectController.Instance.hungry.Play();
        }

        void HandleEndDayFinish_StaminaOut(Unit _)
        {
            //_animator.SetLayerWeight(_animator.GetLayerIndex($"EndDay"), 0);
            _animator.SetInteger("EndDayState", 0);
        }



        void HandleEndDay_TimeOut()
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex($"EndDay"), 1f);
            _animator.SetInteger("EndDayState", 2);
        }

        void HandleEndDayFinish_TimeOut(Unit _)
        {
            //_animator.SetLayerWeight(_animator.GetLayerIndex($"EndDay"), 0);
            _animator.SetInteger("EndDayState", 0);
        }

    }

}
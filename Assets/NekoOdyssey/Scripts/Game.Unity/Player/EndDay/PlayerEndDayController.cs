using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.Player.EndDay
{
    public class PlayerEndDayController : MonoBehaviour
    {
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
                .Subscribe(CheckPlayerMode)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Stamina.OnStaminaOutFinish
                .Subscribe(HandleEndDayFinish_StaminaOut)
                .AddTo(this);
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
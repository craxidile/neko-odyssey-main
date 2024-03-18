using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Unit = UniRx.Unit;

namespace NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.CallToFeed
{
    public class CallToFeedController : MonoBehaviour
    {
        private bool _executing;

        private CatAi _catAi;
        private CatBehaviourController _behaviourController;
        private Animator _animator;

        private void Awake()
        {
            var behaviourController = GetComponent<CatBehaviourController>();
            _catAi = behaviourController.CatAi;
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _catAi.OnCallToFeed
                .Subscribe(HandleCallToFeed)
                .AddTo(this);
        }

        private void HandleCallToFeed(Unit _)
        {
            if (_executing) return;
            _executing = true;
            _animator.SetBool($"Move", false);
            DOVirtual.DelayedCall(.1f, () =>
            {
                _animator.SetBool($"Hungry", true);
            });
            DOVirtual.DelayedCall(Random.Range(.8f, 2f), () =>
            {
                _animator.SetBool($"Hungry", false);
                _executing = false;
            });
        }
        
    }
}
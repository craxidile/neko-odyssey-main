using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;
using Unit = UniRx.Unit;

namespace NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.CallToFeed
{
    public class CallToFeedController : MonoBehaviour
    {
        private bool _executing;

        private CatAi _catAi;
        private AnimatorController _animatorController;
        private CatBehaviourController _behaviourController;

        private void Awake()
        {
            _animatorController = GetComponent<AnimatorController>();
            var behaviourController = GetComponent<CatBehaviourController>();
            _catAi = behaviourController.CatAi;
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
            DOVirtual.DelayedCall(Random.Range(0.5f, 2f), () =>
            {
                _executing = false;
            });
        }
        
    }
}
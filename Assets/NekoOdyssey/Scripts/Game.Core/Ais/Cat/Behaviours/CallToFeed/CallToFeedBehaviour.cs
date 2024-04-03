using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.CallToFeed
{
    public class CallToFeedBehaviour : BaseCatBehaviour
    {
        private const float EligibleDistanceFromPlayer = 1f;
        private const float CoolDownConstantDelay = 0f;
        private const float BackoffDelay = 1f;

        private bool _calling;
        private IDisposable _playerDistanceSubscription;

        public override float CoolDownDelay => CoolDownConstantDelay;

        public CallToFeedBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            //Debug.Log(">>modes<< call_to_feed start");
            _calling = false;
            _playerDistanceSubscription = CatAi
                .OnChangePlayerDistance
                .Subscribe(HandlePlayerDistance)
                .AddTo(GameRunner.Instance);
        }

        private void HandlePlayerDistance(float distance)
        {
            if (_calling || CatAi.Mode != CatBehaviourMode.CallToFeed) return;
            _calling = true;
            //Debug.Log(">>modes<< call_to_feed operate");
            var playerInRange = distance <= EligibleDistanceFromPlayer;
            if (!playerInRange || !IsExecutable)
            {
                DOVirtual.DelayedCall(BackoffDelay, End);
                return;
            }
            CatAi.OnFlip.OnNext(CatAi.DeltaXFromPlayer > 0);
            var delay = Random.Range(2f, 4f);
            CatAi.OnCallToFeed.OnNext(delay);
            CoolDown(0);
            DOVirtual.DelayedCall(delay + BackoffDelay, End);
        }

        private void End()
        {
            _playerDistanceSubscription.Dispose();
            //Debug.Log(">>modes<< call_to_feed end");
            _calling = false;
            CatAi.OnFinishBehaviour.OnNext(CatBehaviourMode.CallToFeed);
        }
    }
}

using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.CallToFeed
{
    public class CallToFeedBehaviour : BaseCatBehaviour
    {
        private const float EligibleDistanceFromPlayer = 1f;
        private const float CoolDownConstantDelay = 0f;

        private bool _calling;
        private IDisposable _playerDistanceSubscription;

        public override float CoolDownDelay => CoolDownConstantDelay;

        public CallToFeedBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            Debug.Log(">>modes<< call_to_feed start");
            _calling = false;
            _playerDistanceSubscription = CatAi.OnChangePlayerDistance
                .Subscribe(HandlePlayerDistance)
                .AddTo(GameRunner.Instance);
        }

        private void HandlePlayerDistance(float distance)
        {
            if (_calling || CatAi.Mode != CatBehaviourMode.CallToFeed) return;
            _calling = true;
            Debug.Log(">>modes<< call_to_feed operate");
            var playerInRange = distance <= EligibleDistanceFromPlayer;
            if (!playerInRange || !IsExecutable)
            {
                End();
                return;
            }
            CatAi.OnFlip.OnNext(CatAi.DeltaXFromPlayer > 0);
            CatAi.OnCallToFeed.OnNext(default);
            CoolDown(0);
            DOVirtual.DelayedCall(4f, End);
        }

        private void End()
        {
            _playerDistanceSubscription.Dispose();
            Debug.Log(">>modes<< call_to_feed end");
            _calling = false;
            CatAi.OnFinishBehaviour.OnNext(Unit.Default);
        }
    }
}
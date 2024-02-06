using System.Collections;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.CallToFeed
{
    public class CallToFeedBehaviour : BaseCatBehaviour
    {
        private const float EligibleDistanceFromPlayer = 1f;

        public override float CoolDownDelay => 0f;

        public CallToFeedBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            CatAi.OnChangePlayerDistance
                .Subscribe(HandlePlayerDistance)
                .AddTo(GameRunner.Instance);
        }
        
        private void HandlePlayerDistance(float distance)
        {
            var playerInRange = distance <= EligibleDistanceFromPlayer;
            if (!playerInRange || !IsExecutable) return;
            CatAi.OnFlip.OnNext(CatAi.DeltaXFromPlayer > 0);
            CatAi.OnCallToFeed.OnNext(default);
            CoolDown(0);
        }
        
    }
}
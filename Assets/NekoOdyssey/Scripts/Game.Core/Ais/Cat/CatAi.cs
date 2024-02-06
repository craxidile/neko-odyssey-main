using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.CallToFeed;
using NekoOdyssey.Scripts.Models;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat
{
    public class CatAi
    {
        public float DeltaXFromPlayer { get; private set; }
        
        public float PlayerDistance { get; private set; } = 0;
        public CatProfile Profile { get; private set; }
        public List<ICatBehaviour> Behaviours { get; } = new();

        public Subject<float> OnChangePlayerDistance { get; } = new();
        public Subject<bool> OnFlip { get; } = new();
        public Subject<Unit> OnCallToFeed { get; } = new();
        
        public CatAi(CatProfile profile)
        {
            Profile = profile;
            if (Profile.HasCallToFeedBehaviour)
            {
                Behaviours.Add(new CallToFeedBehaviour(this));
            }
        }
        
        public void Bind()
        {
        }

        public void Start()
        {
            foreach (var behaviour in Behaviours)
                behaviour.Start();
        }

        public void Unbind()
        {
        }

        public void SetPlayerDistance(float distance, float deltaX)
        {
            PlayerDistance = distance;
            DeltaXFromPlayer = deltaX;
            OnChangePlayerDistance.OnNext(distance);
        }
    }
}
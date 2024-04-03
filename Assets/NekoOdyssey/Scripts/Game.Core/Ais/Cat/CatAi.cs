using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.CallToFeed;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.FollowPlayer;
using NekoOdyssey.Scripts.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat
{
    public class CatAi
    {
        public float DeltaXFromPlayer { get; private set; }
        public float PlayerDistance { get; private set; } = 0;
        public Vector3 CatPosition { get; private set; } = Vector3.zero;
        public CatProfile Profile { get; private set; }
        public CatBehaviourMode Mode { get; private set; } = CatBehaviourMode.None;
        public bool WaitingToWalk { get; set; }
        public Dictionary<CatBehaviourMode, ICatBehaviour> Behaviours { get; } = new();

        public Subject<CatBehaviourMode> OnChangeMode { get; } = new();
        public Subject<Vector3> OnChangeCatPosition { get; } = new();
        public Subject<float> OnChangePlayerDistance { get; } = new();
        public Subject<bool> OnFlip { get; } = new();
        public Subject<float> OnCallToFeed { get; } = new();
        public Subject<Vector3> OnCatMove { get; } = new();
        public Subject<CatBehaviourMode> OnFinishBehaviour { get; } = new();

        public GameObject GameObject { get; set; }
        
        public CatAi(CatProfile profile)
        {
            Profile = profile;
            if (Profile.HasCallToFeedBehaviour)
            {
                Behaviours.Add(CatBehaviourMode.CallToFeed, new CallToFeedBehaviour(this));
                Behaviours.Add(CatBehaviourMode.FollowPlayer, new FollowPlayerBehaviour(this));
            }
        }
        
        public void Bind()
        {
        }

        public void Start()
        {
            OnFinishBehaviour
                .Subscribe(HandleBehaviourFinish)
                .AddTo(GameRunner.Instance);
                
            var modes = Behaviours.Keys.ToList();
            Debug.Log($">>modes<< {modes}");
            var firstMode = modes.FirstOrDefault();
            if (firstMode == CatBehaviourMode.None) return;
            Debug.Log($">>modes<< awake {firstMode}");
            SetMode(firstMode);
            Behaviours[firstMode].Start();
        }

        public void Unbind()
        {
        }

        private void HandleBehaviourFinish(CatBehaviourMode mode)
        {
            if (Behaviours.Count == 1)
            {
                Behaviours.Values.First().Start();
                return;
            }
            var modes = Behaviours.Keys.ToList();
            var modeIndex = modes.IndexOf(Mode);
            var nextIndex = (modeIndex + 1) % modes.Count;
            var nextMode = modes[nextIndex];
            //Debug.Log($">>modes<< finish_behaviour {Mode} next {nextMode}");
            var nextBehaviour = Behaviours[nextMode];
            //Debug.Log($">>modes<< next_behaviour {nextBehaviour}");
            SetMode(nextMode);
            nextBehaviour.Start();
        }

        private void SetMode(CatBehaviourMode mode)
        {
            Debug.Log($">>change_mode<< set {mode}");
            Mode = mode;
            OnChangeMode.OnNext(mode);
        }

        public void SetCatPosition(Vector3 position)
        {
            CatPosition = position;
            OnChangeCatPosition.OnNext(position);
        }

        public void SetPlayerDistance(float distance, float deltaX)
        {
            PlayerDistance = distance;
            DeltaXFromPlayer = deltaX;
            OnChangePlayerDistance.OnNext(distance);
        }
        
    }
}

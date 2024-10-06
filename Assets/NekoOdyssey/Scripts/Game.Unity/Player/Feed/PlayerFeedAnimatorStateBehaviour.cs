using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Cat;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Feed
{
    public class PlayerFeedAnimatorStateBehaviour : StateMachineBehaviour
    {
        private readonly List<int> _eligibleLoopStates = new()
        {
            Animator.StringToHash($"FeedSideSitHighLoop"),
            Animator.StringToHash($"FeedSideSitLoop"),
            Animator.StringToHash($"FeedSideLowLoop"),
            Animator.StringToHash($"FeedSideUpperLowLoop"),
            Animator.StringToHash($"FeedSideMediumLoop"),
            Animator.StringToHash($"FeedSideHighLoop"),
            Animator.StringToHash($"FeedStraightSitLoop"),
            Animator.StringToHash($"FeedStraightSitHighLoop"),
            Animator.StringToHash($"FeedStraightLowLoop"),
            Animator.StringToHash($"FeedStraightUpperLowLoop"),
            Animator.StringToHash($"FeedStraightMediumLoop"),
            Animator.StringToHash($"FeedStraightHighLoop"),
        };

        private readonly List<int> _eligibleFinishStates = new()
        {
            Animator.StringToHash($"FeedSideSitEnd"),
            Animator.StringToHash($"FeedSideSitHighEnd"),
            Animator.StringToHash($"FeedSideLowEnd"),
            Animator.StringToHash($"FeedSideUpperLowEnd"),
            Animator.StringToHash($"FeedSideMediumEnd"),
            Animator.StringToHash($"FeedSideHighEnd"),
            Animator.StringToHash($"FeedStraightSitEnd"),
            Animator.StringToHash($"FeedStraightSitHighEnd"),
            Animator.StringToHash($"FeedStraightLowEnd"),
            Animator.StringToHash($"FeedStraightUpperLowEnd"),
            Animator.StringToHash($"FeedStraightMediumEnd"),
            Animator.StringToHash($"FeedStraightHighEnd"),
        };

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_eligibleLoopStates.Contains(stateInfo.shortNameHash))
            {
                GameRunner.Instance.Core.Cats.CurrentCat?.Eat(false);
                return;
            }

            var finished = _eligibleFinishStates.Contains(stateInfo.shortNameHash);
            if (!finished) return;
            GameRunner.Instance.Core.Player.Feed.Finish();
        }
    }
}
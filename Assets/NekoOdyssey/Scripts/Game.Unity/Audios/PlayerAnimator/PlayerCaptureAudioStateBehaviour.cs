using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerCaptureAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly List<int> _eligibleStates = new()
        {
            Animator.StringToHash("StartSideCaptureBottom"),
            Animator.StringToHash("StartSideCaptureMiddle"),
            Animator.StringToHash("StartSideCaptureTop"),
            Animator.StringToHash("StartStraightCaptureBottom"),
            Animator.StringToHash("StartStraightCaptureMiddle"),
            Animator.StringToHash("StartStraightCaptureTop"),
        };
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_eligibleStates.Contains(stateInfo.shortNameHash)) return;
            GameRunner.Instance.Core.Audios.ActiveAudio.OnNext("SFX_Capture");
            DOVirtual.DelayedCall(3f, () => GameRunner.Instance.Core.Audios.InactiveAudio.OnNext("SFX_Capture"));
        }
    }
}
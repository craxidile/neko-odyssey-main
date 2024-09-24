using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerMovementAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly int _walkState = Animator.StringToHash("Walk");
        private readonly int _runState = Animator.StringToHash("Run");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _walkState)
                GameRunner.Instance.Core.Audios.ActiveAudio.OnNext("SFX_Walk");
            else if (stateInfo.shortNameHash == _runState)
                GameRunner.Instance.Core.Audios.ActiveAudio.OnNext("SFX_Run");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _walkState)
                GameRunner.Instance.Core.Audios.InactiveAudio.OnNext("SFX_Walk");
            else if (stateInfo.shortNameHash == _runState)
                GameRunner.Instance.Core.Audios.InactiveAudio.OnNext("SFX_Run");
        }
    }
}
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerEndDayAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly int _hungryState = Animator.StringToHash("Hungry");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _hungryState)
            {
                GameRunner.Instance.Core.Audios.AudioToClone.OnNext(("SFX_Hungry", 3f));
            }
        }
    }
}
using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Bag
{
    public class PlayerBagAnimatorStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var finished = stateInfo.shortNameHash == Animator.StringToHash($"CloseBag");
            if (!finished) return;
            // DOVirtual.DelayedCall(.2f, () => GameRunner.Instance.Core.Player.Bag.Finish());
            GameRunner.Instance.Core.Player.Bag.Finish();
        }
    }
}
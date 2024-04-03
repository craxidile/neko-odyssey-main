using System.Linq;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Cats
{
    public class CatStateMachineBehavior : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == Animator.StringToHash($"Walk"))
            {
                Debug.Log($">>walk_ready<< {animator.transform.gameObject}");
                var gameObject = animator.transform.gameObject;
                var eligibleAis = GameRunner.Instance.Core.Ais.CatAis.Where(catAi => catAi.GameObject = gameObject);
                foreach (var ai in eligibleAis) ai.WaitingToWalk = false;
            }
        }
    }
}
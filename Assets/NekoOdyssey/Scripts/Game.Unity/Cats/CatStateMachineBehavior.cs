using System;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Cats
{
    public class CatStateMachineBehavior : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var gameObject = animator.transform.gameObject;
            var eligibleAis = GameRunner.Instance.Core.Ais.CatAis.Where(catAi => catAi.GameObject = gameObject);

            var catAis = eligibleAis as CatAi[] ?? eligibleAis.ToArray();
            foreach (var ai in catAis) ai.SetWalkable(stateInfo.shortNameHash == Animator.StringToHash($"Walk"));
            
            if (stateInfo.shortNameHash == Animator.StringToHash($"Stand"))
            {
                foreach (var ai in catAis) ai.SetReadyToWalk(true);
            }
        }
    }
}
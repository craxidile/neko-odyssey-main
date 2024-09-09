using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviorAction_Stand : NpcBehaviorAction
{
    enum StandDirection { SameAsBefore, Right, Left }
    [SerializeField] StandDirection standDirection;
    [SerializeField] float duration = 1;
    public override IEnumerator Action(NpcBehaviorController npcMain)
    {
        npcMain.animator.runtimeAnimatorController = animator != null ? animator : npcMain.idleAnimator;

        if (standDirection != StandDirection.SameAsBefore)
        {
            var flipx = standDirection == StandDirection.Right;
            if (npcMain.mirrorFlipx)
                flipx = !flipx;
            npcMain.spriteRenderer.flipX = flipx;
        }

        yield return new WaitForSeconds(duration);
    }
}

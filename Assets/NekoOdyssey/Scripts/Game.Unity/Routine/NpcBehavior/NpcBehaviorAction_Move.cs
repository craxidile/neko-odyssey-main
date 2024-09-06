using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviorAction_Move : NpcBehaviorAction
{
    public override IEnumerator Action(NpcBehaviorController npcMain)
    {
        npcMain.animator.runtimeAnimatorController = animator;
        npcMain.spriteRenderer.ScreenFlip(this.transform.position, npcMain.mirrorFlipx);

        while (Vector3.Distance(npcMain.transform.position, this.transform.position) > 0)
        {
            var stepDistance = npcMain.moveSpeed * Time.deltaTime;
            var distanceToTarget = Vector3.Distance(npcMain.transform.position, this.transform.position);

            if (distanceToTarget > stepDistance)
            {
                var direction = (this.transform.position - npcMain.transform.position).normalized;
                npcMain.transform.Translate(direction * stepDistance, Space.World);
            }
            else
            {
                npcMain.transform.position = this.transform.position;
            }

            yield return null;
        }
    }
}

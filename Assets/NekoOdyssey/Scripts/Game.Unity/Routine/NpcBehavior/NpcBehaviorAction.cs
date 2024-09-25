using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcBehaviorAction : MonoBehaviour
{
    public RuntimeAnimatorController animator;

    public abstract IEnumerator Action(NpcBehaviorController npcMain);
}

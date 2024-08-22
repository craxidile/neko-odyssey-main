using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPoint_AutoTrigger : MonoBehaviour
{
    [SerializeField] float triggerDistance = 1f;

    public UnityAction OnTrigger { get; set; }




    private void OnDrawGizmosSelected()
    {
        if (triggerDistance > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
}

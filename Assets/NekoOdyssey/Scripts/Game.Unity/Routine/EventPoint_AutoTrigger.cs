using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPoint_AutoTrigger : MonoBehaviour
{
    [SerializeField] float triggerDistance = 1f;

    public UnityAction OnTrigger { get; set; }

    bool isAvaliable = true;

    private void Update()
    {
        var distance = Vector3.Distance(GameRunner.Instance.Core.Player.GameObject.transform.position, transform.position);
        if (distance <= triggerDistance && isAvaliable)
        {
            isAvaliable = false;

            GetComponent<EventPointInteractive>().Interactive();
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (triggerDistance > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
}

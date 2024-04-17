using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikiUpdateAnimator : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public enum poseType { stand_frontside, stand_frontside_no_shoes, sit_frontside }
    public poseType pose;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (pose == poseType.stand_frontside)
        {
            animator.SetTrigger("stand_frontside");
        }
        else if (pose == poseType.stand_frontside_no_shoes)
        {
            animator.SetTrigger("stand_frontside_no_shoes");
        }
        else if (pose == poseType.sit_frontside)
        {
            animator.SetTrigger("sit_frontside");
        }
    }
    private void Update()
    {
        if (pose == poseType.stand_frontside)
        {
            animator.SetTrigger("stand_frontside");
        }
        else if (pose == poseType.stand_frontside_no_shoes)
        {
            animator.SetTrigger("stand_frontside_no_shoes");
        }
        else if (pose == poseType.sit_frontside)
        {
            animator.SetTrigger("sit_frontside");
        }
    }
}

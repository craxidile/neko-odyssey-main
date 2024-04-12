using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcUpdateAnimator : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public enum poseType { stand_frontside, sit_frontside, sit_back, sit_back_face, work_look }
    public poseType pose;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (pose == poseType.stand_frontside)
        {
            animator.SetTrigger("stand_frontside");
        }
        else if (pose == poseType.sit_frontside)
        {
            animator.SetTrigger("sit_frontside");
        }
        else if (pose == poseType.sit_back)
        {
            animator.SetTrigger("sit_back");
        }
        else if (pose == poseType.sit_back_face)
        {
            animator.SetTrigger("sit_back_face");
        }
        else if (pose == poseType.work_look)
        {
            animator.SetTrigger("work_look");
        }
    }
    private void Update()
    {
        if (pose == poseType.stand_frontside)
        {
            animator.SetTrigger("stand_frontside");
        }
        else if (pose == poseType.sit_frontside)
        {
            animator.SetTrigger("sit_frontside");
        }
        else if (pose == poseType.sit_back)
        {
            animator.SetTrigger("sit_back");
        }
        else if (pose == poseType.sit_back_face)
        {
            animator.SetTrigger("sit_back_face");
        }
        else if (pose == poseType.work_look)
        {
            animator.SetTrigger("work_look");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUpdateAnimator : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public enum poseType { stand_frontside, sit_frontside, stand_hungry, sit_hungry }
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
        else if (pose == poseType.stand_hungry)
        {
            animator.SetTrigger("stand_hungry)");
        }
        else if (pose == poseType.sit_hungry)
        {
            animator.SetTrigger("sit_hungry");
        }
    }
    void Update()
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
        else if (pose == poseType.stand_hungry)
        {
            animator.SetTrigger("stand_hungry)");
        }
        else if (pose == poseType.sit_hungry)
        {
            animator.SetTrigger("sit_hungry");
        }
    }
}

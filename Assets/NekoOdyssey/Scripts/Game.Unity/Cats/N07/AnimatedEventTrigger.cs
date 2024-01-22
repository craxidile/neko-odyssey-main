using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Cats.N07
{
    public class AnimatedEventTrigger : MonoBehaviour
    {
        public List<GameObject> otherEvent = new List<GameObject>();
        public Animator animator;

        public bool takePhoto;
        public bool hideAfterTrigger;
        public bool haveToInteractive;
        private bool canActive;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && canActive)
            {
                Debug.Log("Active now");
                animator.Play("TriggerState");
                takePhoto = true;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AfterState"))
            {
                if (otherEvent.Count > 0)
                {
                    for (var i = 0; i < otherEvent.Count; i++)
                    {
                        otherEvent[i].SetActive(true);
                    }

                    otherEvent.Clear();
                }

                if (hideAfterTrigger)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($">>trigger_enter<< {other} {other.CompareTag("Player")}");
            if (other.CompareTag("Player") && !haveToInteractive)
            {
                animator.Play("TriggerState");
            }

            if (other.CompareTag("Player") && haveToInteractive && !canActive && !takePhoto)
            {
                Debug.Log("Can E active");
                canActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && haveToInteractive && canActive)
            {
                Debug.Log("Can't active");
                canActive = false;
            }
        }
    }
}
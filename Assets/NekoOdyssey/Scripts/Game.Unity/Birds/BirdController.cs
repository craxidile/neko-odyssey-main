using System.Collections;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Birds
{
    public class BirdController : MonoBehaviour
    {
        public float distanceTrigger = 1;
        public float timeCounter = 5f;
        public float speed = 1;

        private Vector3 characterPos;
        private Animator anim;
        private SpriteRenderer spriteRenderer;

        [SerializeField] private bool isTrigger;

        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isTrigger)
            {
                transform.position = Vector3.MoveTowards(transform.position, characterPos, speed * Time.deltaTime) +
                                     Vector3.up * speed * 2.5f * Time.deltaTime;
                anim.SetBool("Flying", true);
                StartCoroutine(SetHideBird(timeCounter));
            }
            else
            {
                FindCharacterTag("Cat");
                FindCharacterTag("Player");
                anim.SetBool("Flying", false);
            }
        }

        private void FindCharacterTag(string tag)
        {
            var arr = GameObject.FindGameObjectsWithTag(tag);
            var pos = transform.position;
            foreach (var target in arr)
            {
                var dist = (target.transform.position - pos).sqrMagnitude;

                var direction = target.gameObject.transform.position - pos;

                //var angle = Vector3.Angle(transform.forward, direction);
                if (dist < distanceTrigger)
                {
                    //nearest = target;
                    //distanceTrigger = dist;
                    characterPos = transform.position - direction;
                    isTrigger = true;
                    FlipSprite(target.gameObject.transform.position);
                }
            }
        }

        public void FlipSprite(Vector3 target)
        {
            //Debug.Log("target" + target);
            var pos = transform.position;
            //Debug.Log("pos" + pos);
            var rot = transform.eulerAngles.y;
            if (rot == 0)
            {
                if (target.x < pos.x)
                {
                    spriteRenderer.flipX = true;
                }

                if (target.x > pos.x)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if (rot == 90 || rot == -270)
            {
                if (target.z < pos.z)
                {
                    spriteRenderer.flipX = false;
                }

                if (target.z > pos.z)
                {
                    spriteRenderer.flipX = true;
                }
            }
            else if (rot == 180 || rot == -180)
            {
                if (target.x < pos.x)
                {
                    spriteRenderer.flipX = false;
                }

                if (target.x > pos.x)
                {
                    spriteRenderer.flipX = true;
                }
            }
            else if (rot == -90 || rot == 270)
            {
                if (target.z < pos.z)
                {
                    spriteRenderer.flipX = true;
                }

                if (target.z > pos.z)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }

        IEnumerator SetHideBird(float time)
        {
            yield return new WaitForSeconds(time);
            this.gameObject.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, distanceTrigger);
        }
    }
}
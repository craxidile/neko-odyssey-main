using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NekoOdyssey.Scripts;
using UniRx;
using System.Linq;

public class NpcBehaviorController : MonoBehaviour
{
    public bool isAvaliable { get; set; } = true;

    public float moveSpeed = 3;
    [Tooltip("duration after conversation")] public float waitDuration = 2f;

    public Transform npcActionRoot;
    List<NpcBehaviorAction> npcActionList;

    public bool autoStart = true;
    public bool circleLoop = false;

    public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController idleAnimator;
    public RuntimeAnimatorController conversationAnimator;
    public bool mirrorFlipx;

    int _currentActionIndex = 0;
    bool _isReversing = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        if (idleAnimator == null) idleAnimator = animator.runtimeAnimatorController;
        if (conversationAnimator == null) conversationAnimator = animator.runtimeAnimatorController;

        SetUpNpcActionList();
        npcActionRoot.SetParent(null);
        npcActionRoot.name = $"{name} {npcActionRoot.name}";

        if (npcActionList.Count > 0)
        {
            if (autoStart)
            {
                StartCoroutine(StartAction());
            }
        }

        var eventPoint = GetComponent<EventPoint>() ?? GetComponentInParent<EventPoint>() ?? GetComponentInChildren<EventPoint>();
        GameRunner.Instance.Core.RoutineManger.OnBeginEventPoint.Subscribe(questEventPoint =>
        {
            if (questEventPoint == eventPoint)
            {
                StopAllCoroutines();
                foreach (var action in npcActionList)
                {
                    action.StopAllCoroutines();
                }

                isAvaliable = false;
                animator.runtimeAnimatorController = conversationAnimator;
            }
        }).AddTo(GameRunner.Instance);
        GameRunner.Instance.Core.RoutineManger.OnCompleteEventPoint.Subscribe(questEventPoint =>
        {
            if (questEventPoint == eventPoint)
            {
                isAvaliable = true;
                animator.runtimeAnimatorController = idleAnimator;
                StartCoroutine(StartAction());
            }
        }).AddTo(GameRunner.Instance);
    }

    void SetUpNpcActionList()
    {
        if (npcActionRoot == null)
        {
            npcActionList = new();
        }
        else
        {
            npcActionList = npcActionRoot.GetComponentsInChildren<NpcBehaviorAction>().ToList();
        }
    }

    IEnumerator StartAction()
    {
        yield return new WaitForSeconds(waitDuration);
        if (_isReversing)
        {
            StartCoroutine(ActionReverse());
        }
        else
        {
            StartCoroutine(ActionFoward());
        }
    }
    IEnumerator ActionFoward()
    {
        _isReversing = false;

        if (npcActionList.Count > _currentActionIndex)
        {
            var targetAction = npcActionList[_currentActionIndex];

            yield return StartCoroutine(targetAction.Action(this));

            _currentActionIndex++;
            StartCoroutine(ActionFoward());
        }
        else
        {
            if (circleLoop)
            {
                _currentActionIndex = 0;
                StartCoroutine(ActionFoward());
            }
            else
            {
                StartCoroutine(ActionReverse());
            }
        }



    }
    IEnumerator ActionReverse()
    {
        _isReversing = true;

        if (_currentActionIndex > 0)
        {
            _currentActionIndex--;
            var targetAction = npcActionList[_currentActionIndex];

            yield return StartCoroutine(targetAction.Action(this));

            StartCoroutine(ActionReverse());
        }
        else
        {
            StartCoroutine(ActionFoward());
        }
    }
    IEnumerator MoveToTarget(NpcMove targetNpcMove)
    {
        animator.runtimeAnimatorController = targetNpcMove.animator;
        spriteRenderer.ScreenFlip(targetNpcMove.target.position, mirrorFlipx);

        var target = targetNpcMove.target;
        while (Vector3.Distance(transform.position, target.position) > 0)
        {
            var stepDistance = moveSpeed * Time.deltaTime;
            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget > stepDistance)
            {
                var direction = (target.position - transform.position).normalized;
                transform.Translate(direction * stepDistance, Space.World);
            }
            else
            {
                transform.position = target.position;
            }

            yield return null;
        }
    }
    IEnumerator ResetToIdle()
    {
        yield return null;
        //yield return new WaitForSeconds(targetNpcMove.afterStepWaitDuration);
    }


    private void OnDrawGizmosSelected()
    {
        SetUpNpcActionList();

        var npcMoveList = npcActionList.Where(action => action is NpcBehaviorAction_Move).ToList();
        if (npcMoveList.Count >= 2)
        {
            Vector3 previousPosition = npcMoveList[0].transform.position;
            for (int i = 1; i < npcMoveList.Count; i++)
            {
                var targetAction = npcMoveList[i];
                Vector3 newPosition = targetAction.transform.position;
                Gizmos.DrawLine(previousPosition, newPosition);
                previousPosition = newPosition;
            }

            if (circleLoop)
            {
                Gizmos.DrawLine(npcMoveList.First().transform.position, npcMoveList.Last().transform.position);
            }
        }
    }
}

[System.Serializable]
public class NpcMove
{
    public Transform target;
    public float afterStepWaitDuration;
    public RuntimeAnimatorController animator;
}
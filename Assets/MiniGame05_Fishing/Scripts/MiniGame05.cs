using System.Collections;
using NekoOdyssey.Scripts;
using UnityEngine;

namespace MiniGame05_Fishing.Scripts
{
    public class MiniGame05 : MonoBehaviour
    {
        public static MiniGame05 Instance { get; private set; }

        [Header("Game Feed")] [Range(1f, 5f)] [SerializeField]
        public int difficult;

        [SerializeField] public GameObject[] baseSpace;
        [SerializeField] public RectTransform hookRect;
        [SerializeField] public RectTransform fishRect;
        [SerializeField] public GameObject alert;

        [Header("Progress")] [SerializeField] public float catchTimer;
        [SerializeField] public float lineSnapTimer;
        [SerializeField] public bool _isGameEnd;
        [SerializeField] public bool _isGameStart;

        [Header("UIs")] [SerializeField] public Transform fishSpace;
        [SerializeField] public Transform startPoint;
        [SerializeField] public Transform endPoint;
        [SerializeField] public Transform hook;
        [SerializeField] public GameObject ready, go, clear, fail;

        private float fishPosition;
        private float fishDestination;
        private float fishSpeed;
        private float fishTimer;

        [Header("Hook&Fish Property")] [SerializeField]
        public float hookPower;

        [SerializeField] public float hookProgressDegradationPower;
        float hookProgress;

        //Change base on difficulty
        [SerializeField] public float timeMultiplier = 3f; //time per move for fish
        [SerializeField] public float smoothMotion = 1f; //how quick it move


        [Header("Player")] [SerializeField] public GameObject fishGauge;
        [SerializeField] public GameObject mikiFishSit, mikiFishStart, mikiFishFight, mikiFishCatch, mikiFishSnap;

        [Header("Audio")] public MiniGame05AudioManager audioManager;

        private void Awake()
        {
            Instance = this;

            ready = GameObject.Find("Ready");
            ready.SetActive(false);
            go = GameObject.Find("Go");
            go.SetActive(false);
            clear = GameObject.Find("Clear");
            clear.SetActive(false);
            fail = GameObject.Find("Fail");
            fail.SetActive(false);
            alert.SetActive(false);
        }

        void Start()
        {
            var connector = MiniGameRunner.Instance.Connector;
            connector.EnterSite();
            
            fishGauge.SetActive(false);
            mikiFishCatch.SetActive(false);
            mikiFishFight.SetActive(false);
            mikiFishStart.SetActive(false);
            mikiFishSnap.SetActive(false);
            StartCoroutine(GameStartSequence());
        }


        void Update()
        {
            if (_isGameStart && !_isGameEnd)
            {
                FishMovement();
                HookProgress();
                CheckOverlap();
                //CheckProgress();
            }
        }

        private void CheckOverlap()
        {
            if (IsRectOverlap(hookRect, fishRect))
            {
                catchTimer += Time.deltaTime;
            }
            else
            {
                lineSnapTimer += Time.deltaTime;
            }
        }

        private bool IsRectOverlap(RectTransform indicator, RectTransform boundary)
        {
            return GetWorldRect(boundary).Contains(GetWorldRect(indicator).center);
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            return new Rect(
                corners[0].x,
                corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y
            );
        }

        private void HookProgress()
        {
            if (Input.GetKeyDown(KeyCode.Space)) hookProgress += hookPower;

            hookProgress -= hookProgressDegradationPower * Time.deltaTime;
            hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);

            hook.position = Vector3.Lerp(startPoint.position, endPoint.position, hookProgress);
        }

        private void FishMovement()
        {
            fishTimer -= Time.deltaTime;
            if (fishTimer < 0)
            {
                fishTimer = Random.value * timeMultiplier;
                fishDestination = Random.value;
            }

            fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
            fishSpace.position = Vector3.Lerp(startPoint.position, endPoint.position, fishPosition);
        }

        private IEnumerator GameStartSequence()
        {
            ready.SetActive(true);
            yield return new WaitForSeconds(2f);
            ready.SetActive(false);
            go.SetActive(true);
            yield return new WaitForSeconds(.7f);
            go.SetActive(false);

            yield return new WaitForSeconds(Random.Range(2.8f, 5f));
            alert.SetActive(true);

            while (!Input.GetKeyDown(KeyCode.Space)) yield return null;

            alert.SetActive(false);
            mikiFishSit.SetActive(false);
            mikiFishStart.SetActive(true);
            yield return new WaitForSeconds(1.471f);
            SetupFishParameter();
            mikiFishFight.SetActive(true);
            mikiFishStart.SetActive(false);
            _isGameStart = true;
            StartCoroutine(CheckProgress());
        }

        private void SetupFishParameter()
        {
            fishRect = baseSpace[difficult - 1].gameObject.GetComponent<RectTransform>();
            fishSpace = baseSpace[difficult - 1].transform;
            fishGauge.SetActive(true);
            baseSpace[difficult - 1].SetActive(true);

            timeMultiplier = 4f - (difficult / 2f);
            smoothMotion = 1.6f - (difficult / 5f);
        }

        IEnumerator CheckProgress()
        {
            while (!_isGameEnd)
            {
                if (catchTimer >= 8f)
                {
                    Debug.Log("Catch!!");
                    _isGameEnd = true;
                    mikiFishCatch.SetActive(true);
                    mikiFishFight.SetActive(false);
                    fishGauge.SetActive(false);
                    yield return new WaitForSeconds(1.583f);
                    clear.SetActive(true);
                    var reward = MiniGame05RewardManager.Instance;
                    reward.RandomReward(difficult - 1);
                    break;
                }

                if (lineSnapTimer >= 10f)
                {
                    Debug.Log("Line Snap!");
                    _isGameEnd = true;
                    mikiFishSnap.SetActive(true);
                    mikiFishFight.SetActive(false);
                    fishGauge.SetActive(false);
                    yield return new WaitForSeconds(3.583f);
                    fail.SetActive(true);
                    break;
                }

                yield return null;
            }
            var connector = MiniGameRunner.Instance.Connector;
            connector.LeaveSite();
        }
        
    }
}
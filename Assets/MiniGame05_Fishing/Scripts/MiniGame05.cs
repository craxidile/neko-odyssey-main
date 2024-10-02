using System;
using System.Collections;
using DG.Tweening;
using NekoOdyssey.Scripts;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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

        [Header("Input Actions")]
        public InputActionReference pullInput;
        public InputActionReference anyKeyInput;

        public Subject<string> OnPlaySfx { get; } = new();
        public Subject<string> OnPlayLoopSfx { get; } = new();
        public Subject<string> OnPlayCloneSfx { get; } = new();
        public Subject<string> OnStopSfx { get; } = new();

        private void Awake()
        {
            Instance = this;

            ready = GameObject.Find("Ready");
            go = GameObject.Find("Go");
            clear = GameObject.Find("Clear");
            fail = GameObject.Find("Fail");
            
            ready.SetActive(false);
            go.SetActive(false);
            clear.SetActive(false);
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

        private static Rect RectTransformToScreenRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = Camera.main.WorldToScreenPoint(corners[i]);
                corners[i] = new Vector3(Mathf.RoundToInt(corners[i].x), Mathf.RoundToInt(corners[i].y), corners[i].z);
            }

            var minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            var minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
            var width = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x) - minX;
            var height = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y) - minY;

            return new Rect(minX, minY, width, height);
        }

        private bool IsRectOverlap(RectTransform indicator, RectTransform boundary)
        {
            var worldBoundary = RectTransformToScreenRect(boundary);
            var worldIndicator = RectTransformToScreenRect(indicator);
            Debug.Log($">>rect<< {worldBoundary.Contains(worldIndicator.center)} {worldIndicator} {worldBoundary}");
            return worldBoundary.Contains(worldIndicator.center);
        }

        private void HookProgress()
        {
            // if (Input.GetKeyDown(KeyCode.Space)) hookProgress += hookPower;
            if (pullInput.action.triggered)
            {
                hookProgress += hookPower;
                OnPlayCloneSfx.OnNext("SFX_Tab");
            }

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
            yield return new WaitForSeconds(2.5f);
            ready.SetActive(true);
            OnPlaySfx.OnNext("SFX_Ready");
            yield return new WaitForSeconds(2f);
            ready.SetActive(false);
            go.SetActive(true);
            OnPlaySfx.OnNext("SFX_Go");
            yield return new WaitForSeconds(.7f);
            go.SetActive(false);

            yield return new WaitForSeconds(Random.Range(2.8f, 5f));
            alert.SetActive(true);
            OnPlayLoopSfx.OnNext("SFX_Alert");

            // while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
            while (!anyKeyInput.action.triggered) yield return null;

            alert.SetActive(false);
            OnStopSfx.OnNext("SFX_Alert");
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
                    OnPlaySfx.OnNext("SFX_Clear");
                    var reward = MiniGame05RewardManager.Instance;
                    reward.RandomReward(difficult);
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
                    OnPlaySfx.OnNext("SFX_Fail");
                    break;
                }

                yield return null;
            }

            
            var connector = MiniGameRunner.Instance.Connector;
            OnStopSfx.OnNext("BGM");
            connector.LeaveSite();
        }
        
    }
}
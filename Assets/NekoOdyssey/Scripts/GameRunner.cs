using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Core.Routine;
using NekoOdyssey.Scripts.Game.Inputs;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Audios.PlayMenu;
using NekoOdyssey.Scripts.Game.Unity.Cameras;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Feed;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using NekoOdyssey.Scripts.Game.Unity.Petting;
using NekoOdyssey.Scripts.Game.Unity.Sites;
using NekoOdyssey.Scripts.Game.Unity.SkipTime;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NekoOdyssey.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        public static GameRunner Instance;

        private PlayerInputActions _inputActions;
        public static readonly Dictionary<string, Object> StaticAssetMap = new();

        public GameCameras cameras;
        public GameCoreMode gameCoreMode = GameCoreMode.All;

        public GameCore Core { get; } = new();

        public PlayerInputHandler PlayerInputHandler { get; private set; }
        public UiInputHandler UiInputHandler { get; private set; }
        public Dictionary<string, Object> AssetMap => StaticAssetMap;
        public bool Ready { get; private set; } = false;

        public Subject<bool> OnReady { get; } = new();


        public CSVHolderScriptable CsvHolder; //Linias Edit**
        public TimeRoutine TimeRoutine { get; private set; }
        public Subject<UniRx.Unit> OnUpdate { get; } = new();

        public GameRunner()
        {
            Instance = this;
        }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            PlayerInputHandler = gameObject.AddComponent<PlayerInputHandler>();
            PlayerInputHandler.InputActions = _inputActions;

            if (gameCoreMode == GameCoreMode.All)
            {
                gameObject.AddComponent<CentralSiteActionController>();
                gameObject.AddComponent<CentralCaptureActionHandler>();
                gameObject.AddComponent<CentralConversationActionHandler>();
                gameObject.AddComponent<CentralPlayerPettingHandler>();
                gameObject.AddComponent<CentralPlayerFeedHandler>();
                gameObject.AddComponent<CentralSkipTimeActionHandler>();
                gameObject.AddComponent<CentralPlayerMenuAudioSwitch>();
                TimeRoutine = gameObject.AddComponent<TimeRoutine>();
            }

            Debug.Log($">>bind_game<< {gameCoreMode}");
            Core.Bind(gameCoreMode);

            //new GameObject("Time Controller").AddComponent<NekoOdyssey.Scripts.Game.Core.Routine.TimeRoutine>().transform.SetParent(transform);
        }

        private void Start()
        {
            Core.Start();
            DOVirtual.DelayedCall(1f, () => SetReady(true));

            if (gameCoreMode == GameCoreMode.All)
            {
                AssetBundleUtils.OnReady(InitializePositions);
            }

            StartCoroutine(IUpdate());
        }

        private void OnDestroy()
        {
            Core.Unbind();
        }
        
        public void SetReady(bool ready)
        {
            Ready = ready;
            OnReady.OnNext(ready);
        }

        private IEnumerator IUpdate()
        {
            OnUpdate.OnNext(UniRx.Unit.Default);
            yield return null;
            StartCoroutine(IUpdate());
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private GameObject LoadCameraBoundaryFromSite(
            Database.Domains.Sites.Entities.SiteEntity.Models.Site currentSite
        )
        {
            if (currentSite == null) return null;
            var cameraBoundaryName = currentSite.CameraBoundary;

            if (cameraBoundaryName == null) return null;
            cameraBoundaryName = cameraBoundaryName.ToLower();

            if (!AssetMap.ContainsKey(cameraBoundaryName)) return null;
            var boundaryGameObject = Instantiate(AssetMap[cameraBoundaryName], transform) as GameObject;

            return boundaryGameObject == null ? null : boundaryGameObject;
        }

        private void InitializePositions()
        {
            var cameraActive = SiteRunner.Instance.Core.Site.CurrentSite.CameraActive;
            if (!cameraActive)
            {
                Camera.main?.gameObject.SetActive(false);
                return;
            }
                
            var boundary = FindAnyObjectByType<CameraBoundary>()?.gameObject;
            if (boundary == null)
            {
                var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
                boundary = LoadCameraBoundaryFromSite(currentSite);
            }

            if (boundary != null && cameras.mainVirtualCamera != null)
            {
                var confiner = cameras.mainVirtualCamera.GetComponent<CinemachineConfiner>();
                confiner.m_BoundingVolume = boundary.GetComponent<BoxCollider>();
            }

            var cameraAnchor = FindAnyObjectByType<CameraAnchor>();
            if (cameraAnchor != null && cameras.mainVirtualCamera != null)
            {
                cameras.mainVirtualCamera.transform.position = cameraAnchor.transform.position;
            }
        }
    }
}
using System.Collections.Generic;
using Cinemachine;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Cameras;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using NekoOdyssey.Scripts.Game.Unity.Petting;
using NekoOdyssey.Scripts.Game.Unity.Sites;
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

        public GameCameras cameras;

        public GameCore Core { get; } = new();

        public PlayerInputHandler PlayerInputHandler { get; private set; }
        public UiInputHandler UiInputHandler { get; private set; }
        public Dictionary<string, Object> AssetMap { get; } = new();
        public bool Ready { get; private set; } = false;

        public Subject<bool> OnReady { get; } = new();


        public CSVHolderScriptable CsvHolder; //Linias Edit**

        public GameRunner()
        {
            Instance = this;
        }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            PlayerInputHandler = gameObject.AddComponent<PlayerInputHandler>();
            PlayerInputHandler.InputActions = _inputActions;

            gameObject.AddComponent<CentralSiteActionController>();
            gameObject.AddComponent<CentralCaptureActionHandler>();
            gameObject.AddComponent<CentralConversationActionHandler>();
            gameObject.AddComponent<CentralPlayerPettingHandler>();
            gameObject.AddComponent<AssetBundleLoader>();

            new GameObject("Time Controller").AddComponent<NekoOdyssey.Scripts.Game.Core.Routine.TimeRoutine>().transform.SetParent(transform);


            Core.Bind();
        }

        public void SetReady(bool ready)
        {
            Ready = ready;
            OnReady.OnNext(ready);
        }

        private void Start()
        {
            Core.Start();

            AssetBundleUtils.OnReady(InitializePositions);
        }

        private void OnDestroy()
        {
            Core.Unbind();
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

            Debug.Log($">>boundary<< name 01 {cameraBoundaryName}");

            if (!AssetMap.ContainsKey(cameraBoundaryName)) return null;
            var boundaryGameObject = Instantiate(AssetMap[cameraBoundaryName]) as GameObject;
            Debug.Log($">>boundary<< name 02 {boundaryGameObject.name}");

            return boundaryGameObject == null ? null : boundaryGameObject;
        }

        private void InitializePositions()
        {
            Debug.Log($">>boundary<< start");
            var boundary = FindAnyObjectByType<CameraBoundary>()?.gameObject;
            Debug.Log($">>boundary<< 01 {boundary?.GetComponent<BoxCollider>()}");
            if (boundary == null)
            {
                var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
                boundary = LoadCameraBoundaryFromSite(currentSite);
                Debug.Log($">>boundary<< 02 {boundary?.GetComponent<BoxCollider>()}");
            }


            if (boundary != null && Camera.main != null)
            {
                var confiner = Camera.main.GetComponent<CinemachineConfiner>();
                confiner.m_BoundingVolume = boundary.GetComponent<BoxCollider>();
            }

            var cameraAnchor = FindAnyObjectByType<CameraAnchor>();
            if (cameraAnchor != null && Camera.main != null)
            {
                Camera.main.transform.position = cameraAnchor.transform.position;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Cameras;
using NekoOdyssey.Scripts.Game.Unity.Capture;
using NekoOdyssey.Scripts.Game.Unity.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using NekoOdyssey.Scripts.Game.Unity.Sites;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using Object = UnityEngine.Object;

namespace NekoOdyssey.Scripts.Game.Unity
{
    public class GameRunner : MonoBehaviour
    {
        public static GameRunner Instance;

        private PlayerInputActions _inputActions;

        public GameCoreRunner Core { get; } = new();

        public GameObject testCanvas;

        public PlayerInputHandler PlayerInputHandler { get; private set; }
        public UiInputHandler UiInputHandler { get; private set; }
        public Dictionary<string, Object> AssetMap { get; } = new();
        public bool Ready { get; private set; } = false;
        public Subject<bool> OnReady { get; } = new();

        public GameRunner()
        {
            Instance = this;
            Debug.Log($">>game_runner<< ctor");
        }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            PlayerInputHandler = gameObject.AddComponent<PlayerInputHandler>();
            PlayerInputHandler.InputActions = _inputActions;

            gameObject.AddComponent<GlobalSiteEntranceController>();
            gameObject.AddComponent<CentralCaptureActionHandler>();
            gameObject.AddComponent<GlobalConversationActionHandler>();
            gameObject.AddComponent<AssetBundleLoader>();
            
            Core.Bind();
        }

        public void SetReady(bool ready)
        {
            Debug.Log($">>ready<< {ready}");
            Ready = ready;
            OnReady.OnNext(ready);
        }

        private void Start()
        {
            Core.Start();
            
            var boundary = FindAnyObjectByType<CameraBoundary>();
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
    }
}
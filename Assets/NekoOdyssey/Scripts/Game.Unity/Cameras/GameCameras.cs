using UnityEngine;
using Cinemachine;

namespace NekoOdyssey.Scripts.Game.Unity.Cameras
{
    [System.Serializable]
    public struct GameCameras
    {
        public Camera mainCamera;
        public Camera playerCamera;
        public Camera sellerCamera;
        public CinemachineVirtualCamera mainVirtualCamera;
    }
}
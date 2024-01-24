using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Capture
{
    public class GlobalCaptureActionHandler : MonoBehaviour
    {
        private void Start()
        {
            GameRunner.Instance.GameCore.PlayerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {

            if (action != PlayerMenuAction.Camera) return;
            GameRunner.Instance.GameCore.PlayerMenu.SetActive(false);
            
            var menuGameObject = GameRunner.Instance.GameCore.PlayerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<CaptureAttributes>();
            Debug.Log($">>attributes<< {attributes}");
            if (attributes == null) return;

            GameRunner.Instance.GameCore.Player.Capture.TargetPosition = attributes.captureAnchor.transform.position;
            GameRunner.Instance.GameCore.Player.Capture.Mode = attributes.captureMode;
            GameRunner.Instance.GameCore.Player.Capture.CatCode = attributes.catCode;

            GameRunner.Instance.GameCore.Player.SetMode(PlayerMode.Capture);
        }
    }
}
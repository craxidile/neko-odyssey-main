using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Capture
{
    public class CentralCaptureActionHandler : MonoBehaviour
    {
        private void Start()
        {
            GameRunner.Instance.Core.PlayerMenu.OnCommitAction.Subscribe(HandlePlayerMenuAction);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {

            if (action != PlayerMenuAction.Camera) return;
            GameRunner.Instance.Core.PlayerMenu.SetActive(false);

            DOVirtual.DelayedCall(2f, () =>
            {
                SoundEffectController.Instance.shutter.Play();
            });
            
            var menuGameObject = GameRunner.Instance.Core.PlayerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<CaptureAttributes>();
            Debug.Log($">>attributes<< {attributes}");
            if (attributes == null) return;

            GameRunner.Instance.Core.Player.Capture.TargetPosition = attributes.captureAnchor.transform.position;
            GameRunner.Instance.Core.Player.Capture.Mode = attributes.captureMode;
            GameRunner.Instance.Core.Player.Capture.CatCode = attributes.catCode;

            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Capture);
        }
    }
}
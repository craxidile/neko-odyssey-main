using NekoOdyssey.Scripts.Game.Core.Capture;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Player.Capture
{
    public class PlayerCapture
    {
        public Vector3 TargetPosition { get; set; }
        public CaptureMode Mode { get; set; }
        public string CatCode { get; set; }

        public Subject<Unit> OnCaptureBegin { get; } = new();
        public Subject<Unit> OnCaptureFinish { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void Begin()
        {
            OnCaptureBegin.OnNext(default);
        }
        public void Finish()
        {
            GameRunner.Instance.Core.Player.Phone.CatNote.AddCatCapture(CatCode);

            GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteNameActive();
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(0);

            OnCaptureFinish.OnNext(default);
        }
    }
}
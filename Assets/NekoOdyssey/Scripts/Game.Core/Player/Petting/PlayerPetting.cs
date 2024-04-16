using NekoOdyssey.Scripts.Game.Core.Petting;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Petting
{
    public class PlayerPetting
    {
        public Vector3 TargetPosition { get; set; }
        public PettingMode Mode { get; set; }

        public Subject<Unit> OnFinishPetting { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void Finish()
        {
            OnFinishPetting.OnNext(Unit.Default);
            // GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteActive();
            GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteNameActive();
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
        }
    }
}
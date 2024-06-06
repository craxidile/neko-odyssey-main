using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Inputs
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; set; }

        public IObservable<Vector2> OnMove => this.UpdateAsObservable()
            .Select(_ => InputActions.Player.Movement.ReadValue<Vector2>());
        
        public IObservable<Unit> OnRun => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Run.triggered);

        public IObservable<Unit> OnSpeedStart => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Speed.inProgress);

        public IObservable<Unit> OnSpeedEnd => this.UpdateAsObservable()
            .Where(_ => !InputActions.Player.Speed.inProgress);


        public IObservable<Unit> OnPhoneTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Phone.triggered);

        public IObservable<Unit> OnBagTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Bag.triggered);
        
        public IObservable<Unit> OnResetSaveTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.ResetSave.triggered);

        public IObservable<Unit> OnPrevMenuTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.PrevMenu.triggered);
        
        public IObservable<Unit> OnNextMenuTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.NextMenu.triggered);
        
        public IObservable<Unit> OnPrevTabTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.PrevTab.triggered);
        
        public IObservable<Unit> OnNextTabTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.NextTab.triggered);

        public IObservable<Unit> OnFireTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Fire.triggered);

        public IObservable<Unit> OnCancelTriggerred => this.UpdateAsObservable()
            .Where(_ => InputActions.Player.Cancel.triggered);
    }
}
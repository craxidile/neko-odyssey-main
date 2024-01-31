using System;
using System.Linq;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity;
using UniRx;
using UnityEngine;

namespace Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu
{
    public class PlayerMenu
    {
        private bool _active;
        private PlayerMenuAction _currentAction;
        private PlayerMenuAction[] _actions = Array.Empty<PlayerMenuAction>();

        private IDisposable _nextMenuTriggeredSubscription;
        private IDisposable _prevMenuTriggeredSubscription;
        private IDisposable _fireTriggeredSubscription;

        public PlayerMenuSite Site { get; set; } = PlayerMenuSite.None;
        
        public GameObject GameObject { get; set; }

        public Subject<bool> OnActive { get; } = new();
        public Subject<PlayerMenuAction> OnChangeAction { get; } = new();
        public Subject<PlayerMenuAction> OnCommitAction { get; } = new();

        public void SetActive(bool active)
        {
            _active = active;
            OnActive.OnNext(_active);
            OnChangeAction.OnNext(_currentAction);
        }

        public void SetCurrentAction(PlayerMenuAction action)
        {
            _currentAction = action;
            Debug.Log($">>on_change_action<< {action}");
            OnChangeAction.OnNext(action);
        }

        public void SetActions(PlayerMenuAction[] actions)
        {
            _actions = actions;
            if (_actions.Length == 0) return;
            SetCurrentAction(actions[0]);
        }

        public void Reset()
        {
            SetActive(false);
            _currentAction = PlayerMenuAction.None;
            _actions = Array.Empty<PlayerMenuAction>();
            Site = PlayerMenuSite.None;
        }

        public void Bind()
        {
            Reset();
        }

        public void Start()
        {
            _fireTriggeredSubscription = GameRunner.Instance.PlayerInputHandler.OnFireTriggerred.Subscribe(_ =>
            {
                if (!_active || _currentAction == PlayerMenuAction.None) return;
                SetActive(false);
                OnCommitAction.OnNext(_currentAction);
            });
            _nextMenuTriggeredSubscription = GameRunner.Instance.PlayerInputHandler.OnNextMenuTriggerred.Subscribe(_ =>
            {
                if (!_active || _actions.Length == 0) return;
                var index = _actions.ToList().IndexOf(_currentAction);
                index = Math.Min(_actions.Length - 1, index + 1);
                SetCurrentAction(_actions[index]);
            });
            _prevMenuTriggeredSubscription = GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred.Subscribe(_ =>
            {
                if (!_active || _actions.Length == 0) return;
                var index = _actions.ToList().IndexOf(_currentAction);
                index = Math.Max(0, index - 1);
                SetCurrentAction(_actions[index]);
            });
        }

        public void Unbind()
        {
            Reset();
            _fireTriggeredSubscription.Dispose();
            _nextMenuTriggeredSubscription.Dispose();
            _prevMenuTriggeredSubscription.Dispose();
        }
    }
}
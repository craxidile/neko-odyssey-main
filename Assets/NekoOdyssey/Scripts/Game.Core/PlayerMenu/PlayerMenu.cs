using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts;
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

        public PlayerMenuSite Site { get; set; } = PlayerMenuSite.None;
        public int MenuLevel { get; private set; } = 0;
        
        public GameObject GameObject { get; set; }

        public Subject<bool> OnActive { get; } = new();
        public Subject<int> OnChangeMenuLevel { get; } = new();
        public Subject<PlayerMenuAction> OnChangeAction { get; } = new();
        public Subject<PlayerMenuAction> OnCommitAction { get; } = new();

        public void SetActive(bool active)
        {
            _active = active;
            OnActive.OnNext(_active);
            OnChangeAction.OnNext(_currentAction);
            SetMenuLevel(MenuLevel);
        }

        public void SetMenuLevel(int level)
        {
            MenuLevel = level;
            OnChangeMenuLevel.OnNext(MenuLevel);
            Debug.Log($">>menu_level<< actions_length {MenuLevel} {_actions.Length}");
            if (_actions.Length == 0) return;
            var aa = level == 0 && _actions.Length > 1 ? PlayerMenuAction.Exclamation : _actions[0];
            Debug.Log($">>menu_level<< current_action {aa}");
            SetCurrentAction(level == 0 && _actions.Length > 1 ? PlayerMenuAction.Exclamation : _actions[0]);
        }

        public void SetCurrentAction(PlayerMenuAction action)
        {
            _currentAction = action;
            Debug.Log($">>menu_level<< on_change_action {action}");
            OnChangeAction.OnNext(action);
        }

        public void SetActions(PlayerMenuAction[] actions)
        {
            _actions = actions;
            if (_actions.Length == 0) return;
            SetCurrentAction(MenuLevel == 0 && actions.Length > 1 ? PlayerMenuAction.Exclamation : actions[0]);
        }

        public void Reset()
        {
            SetActive(false);
            _currentAction = PlayerMenuAction.None;
            _actions = Array.Empty<PlayerMenuAction>();
            MenuLevel = 0;
            Site = PlayerMenuSite.None;
        }

        public void Bind()
        {
            Reset();
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnFireTriggerred.Subscribe(_ =>
            {
                if (!_active || _currentAction == PlayerMenuAction.None) return;
                SetActive(MenuLevel == 0 && _actions.Length > 1);
                OnCommitAction.OnNext(_currentAction);
            }).AddTo(GameRunner.Instance);
            GameRunner.Instance.PlayerInputHandler.OnNextMenuTriggerred.Subscribe(_ =>
            {
                if (!_active || _actions.Length == 0) return;
                var index = _actions.ToList().IndexOf(_currentAction);
                index = Math.Min(_actions.Length - 1, index + 1);
                SetCurrentAction(_actions[index]);
            }).AddTo(GameRunner.Instance);
            GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred.Subscribe(_ =>
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
        }
    }
}
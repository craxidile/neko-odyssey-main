using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu
{
    public class PlayerMenu
    {
        private readonly List<PlayerMenuAction> _actionsToStopPlayer = new()
        {
            PlayerMenuAction.Enter,
            PlayerMenuAction.Exit,
            PlayerMenuAction.Left,
            PlayerMenuAction.Right,
            PlayerMenuAction.Up,
            PlayerMenuAction.Down,
            PlayerMenuAction.LeftEnter,
            PlayerMenuAction.RightEnter,
            PlayerMenuAction.LeftExit,
            PlayerMenuAction.RightExit
        };

        private bool _active;
        private PlayerMenuAction _currentAction;
        private PlayerMenuAction[] _actions = Array.Empty<PlayerMenuAction>();

        // public PlayerMenuSite Site { get; private set; } = PlayerMenuSite.None;
        public string SiteName { get; private set; }
        public int MenuLevel { get; private set; } = 0;

        public GameObject GameObject { get; set; }

        // public Subject<bool> OnActive { get; } = new();
        // public Subject<PlayerMenuSite> OnChangeSite { get; } = new();
        // public Subject<Tuple<PlayerMenuSite, bool>> OnChangeSiteActive { get; } = new();
        public Subject<int> OnChangeMenuLevel { get; } = new();
        public Subject<PlayerMenuAction> OnChangeAction { get; } = new();
        public Subject<PlayerMenuAction> OnCommitAction { get; } = new();
        public Subject<Tuple<string, bool>> OnChangeSiteNameActive { get; } = new();

        public void SetActive(bool active)
        {
            _active = active;
            //// OnActive.OnNext(_active);
            // OnChangeSiteActive.OnNext(Tuple.Create(Site, active));
            OnChangeSiteNameActive.OnNext(Tuple.Create(SiteName, active));
            OnChangeAction.OnNext(_currentAction);
            SetMenuLevel(MenuLevel);
        }

        // public void SetSiteActive(PlayerMenuSite site, bool active)
        // {
        //     Site = site;
        //     _active = active;
        //     SetMenuLevel(0);
        //     OnChangeSiteActive.OnNext(Tuple.Create(site, active));
        // }

        public void SetSiteNameActive(string siteName, bool active)
        {
            SiteName = !active ? null : siteName;
            _active = active;
            SetMenuLevel(0);
            OnChangeSiteNameActive.OnNext(Tuple.Create(siteName, active));
        }

        public void SetMenuLevel(int level)
        {
            var playerMode = GameRunner.Instance.Core.Player.Mode;
            Debug.Log($"<color=yellow>>>player_mode<< {playerMode} {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name}</color>");
            if (playerMode != PlayerMode.Move && playerMode != PlayerMode.Submenu) return;
            GameRunner.Instance.Core.Player.SetMode(level == 0 ? PlayerMode.Move : PlayerMode.Submenu);
            MenuLevel = level;
            OnChangeMenuLevel.OnNext(MenuLevel);
            //Debug.Log($">>menu_level<< actions_length {MenuLevel} {_actions.Length}");
            if (_actions.Length == 0) return;
            SetCurrentAction(level == 0 && _actions.Length > 1 ? PlayerMenuAction.Exclamation : _actions[0]);
        }

        public void SetCurrentAction(PlayerMenuAction action)
        {
            Debug.Log($">>vel_vel<< current_action {action}");
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

        // public void SetCurrentSiteActive()
        // {
        //     SetSiteActive(Site, true);
        // }

        public void SetCurrentSiteNameActive()
        {
            SetSiteNameActive(SiteName, true);
        }

        public void Reset()
        {
            SetActive(false);
            _currentAction = PlayerMenuAction.None;
            _actions = Array.Empty<PlayerMenuAction>();
            MenuLevel = 0;
            // Site = PlayerMenuSite.None;
            SiteName = null;
        }

        public void Bind()
        {
            Reset();
        }

        public void Start()
        {
            // Commit Action
            GameRunner.Instance.PlayerInputHandler.OnFireTriggerred
                .Subscribe(_ => { CommitAction(); })
                .AddTo(GameRunner.Instance);
            // Cancel Action
            GameRunner.Instance.PlayerInputHandler.OnCancelTriggerred
                .Subscribe(_ => { CancelAction(); })
                .AddTo(GameRunner.Instance);
            // Previous Action
            GameRunner.Instance.PlayerInputHandler.OnNextMenuTriggerred
                .Subscribe(_ => { SelectPreviousAction(); })
                .AddTo(GameRunner.Instance);
            // Next Action
            GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred
                .Subscribe(_ => { SelectNextAction(); })
                .AddTo(GameRunner.Instance);
            // Slide Action
            GameRunner.Instance.PlayerInputHandler.OnSlideAction
                .Subscribe(SlideAction)
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
            Reset();
        }

        private void CancelAction()
        {
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Submenu) return;
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
            // SetCurrentSiteActive();
            SetCurrentSiteNameActive();
        }

        private void CommitAction()
        {
            if (
                GameRunner.Instance.Core.Player.Mode != PlayerMode.Move &&
                GameRunner.Instance.Core.Player.Mode != PlayerMode.Submenu
            ) return;
            if (!_active || _currentAction == PlayerMenuAction.None) return;
            // if (MenuLevel == 0 && _actions.Length > 1)
            //     OnChangeSiteActive.OnNext(Tuple.Create(Site, false));
            var menuLevel = MenuLevel;
            var actionsLength = _actions.Length;
            OnCommitAction.OnNext(_currentAction);
            if (menuLevel > 0 || (menuLevel == 0 && actionsLength == 1))
                // OnChangeSiteActive.OnNext(Tuple.Create(Site, false));
                OnChangeSiteNameActive.OnNext(Tuple.Create(SiteName, false));
            if (_actionsToStopPlayer.Contains(_currentAction))
                GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);
        }

        private void SlideAction(Vector2 movement)
        {
            var mode = GameRunner.Instance.Core.Player.Mode;
            if (mode != PlayerMode.Submenu || !_active || _actions.Length == 0) return;
            var x = movement.x;
            switch (x)
            {
                case > 0:
                    SelectPreviousAction();
                    break;
                case < 0:
                    SelectNextAction();
                    break;
            }
        }

        private void SelectPreviousAction()
        {
            var mode = GameRunner.Instance.Core.Player.Mode;
            if (mode != PlayerMode.Submenu || !_active || _actions.Length == 0) return;
            var index = _actions.ToList().IndexOf(_currentAction);
            index = Math.Min(_actions.Length - 1, index + 1);
            SetCurrentAction(_actions[index]);
        }

        private void SelectNextAction()
        {
            var mode = GameRunner.Instance.Core.Player.Mode;
            if (mode != PlayerMode.Submenu || !_active || _actions.Length == 0) return;
            var index = _actions.ToList().IndexOf(_currentAction);
            index = Math.Max(0, index - 1);
            SetCurrentAction(_actions[index]);
        }
    }
}
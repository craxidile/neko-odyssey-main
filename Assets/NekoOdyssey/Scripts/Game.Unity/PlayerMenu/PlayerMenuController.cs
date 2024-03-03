using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace NekoOdyssey.Scripts.Game.Unity.PlayerMenu
{
    public class PlayerMenuController : MonoBehaviour
    {
        private const float MenuScale = 1f;
        private const float MenuGap = MenuScale * .4f;

        private readonly List<GameObject> _banners = new();
        private bool _eligibleToShow = false;
        private bool _active = false;

        public PlayerMenuAction[] availableActions;
        public PlayerMenuSite site;
        public bool autoActive;

        private void Awake()
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                if (GameRunner.Instance.Ready)
                {
                    LoadBanners();
                }
                else
                {
                    GameRunner.Instance.OnReady.Subscribe(ready =>
                    {
                        if (!ready) return;
                        LoadBanners();
                    });
                }
            });
        }

        private void LoadBanners()
        {
            if (availableActions.Length > 1)
                StartCoroutine(CreateActionBanner(PlayerMenuAction.Exclamation, 0, 1, 0));
            for (var i = 0; i < availableActions.Length; i++)
            {
                var action = availableActions[i];
                StartCoroutine(CreateActionBanner(
                    action,
                    i, availableActions.Length,
                    availableActions.Length == 1 ? 0 : 1)
                );
            }
        }

        private void Start()
        {
            GameRunner.Instance.Core.PlayerMenu.OnActive
                .Subscribe(SetMenuActive)
                .AddTo(this);
            GameRunner.Instance.Core.PlayerMenu.OnChangeAction
                .Subscribe(TriggerCurrentAction)
                .AddTo(this);
            GameRunner.Instance.Core.PlayerMenu.OnChangeMenuLevel
                .Subscribe(SetMenuLevel)
                .AddTo(this);
            GameRunner.Instance.Core.PlayerMenu.OnCommitAction
                .Subscribe(HandlePlayerMenuAction)
                .AddTo(this);
        }

        private void OnTriggerEnter(Collider other) => OnTriggerStay(other);

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _eligibleToShow = true;
            GameRunner.Instance.Core.PlayerMenuCandidateManager.Add(new PlayerMenuCandidate
            {
                Actions = availableActions,
                GameObject = gameObject,
                Site = site,
                AutoActive = autoActive,
                DistanceFromPlayer = Vector3.Distance(other.transform.position, transform.position)
            });
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _eligibleToShow = false;
            var menuCandidateManager = GameRunner.Instance.Core.PlayerMenuCandidateManager;
            menuCandidateManager.Remove(new PlayerMenuCandidate { Site = site });
        }

        private void TriggerCurrentAction(PlayerMenuAction currentAction)
        {
            if (GameRunner.Instance.Core.PlayerMenu.Site != site) return;
            Debug.Log($">>menu_level<< current_action {currentAction}");
            var availableActionList = availableActions.ToList();
            var banners = GameRunner.Instance.Core.PlayerMenu.MenuLevel == 0
                ? _banners.Take(1).ToList()
                : _banners.Skip(1).ToList();
            foreach (var action in availableActionList)
            {
                var index = availableActionList.IndexOf(action);
                if (index < 0 || index >= banners.Count) continue;
                var banner = banners[index];
                if (!banner) continue;
                var animator = banner.GetComponent<Animator>();
                if (!animator) continue;
                animator.SetBool($"Selected", action == currentAction);
            }
        }

        private void SetMenuActive(bool active)
        {
            if (GameRunner.Instance.Core.PlayerMenu.Site != site) return;
            _active = active;
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            if (_active|| action != PlayerMenuAction.Exclamation) return;
            Debug.Log($">>menu_level<< 1");
            GameRunner.Instance.Core.PlayerMenu.SetMenuLevel(1);
        }

        private void SetMenuLevel(int level)
        {
            var bannersToDisplay = _banners
                .Where(banner =>
                {
                    var details = banner.GetComponent<PlayerMenuDetails>();
                    if (!details) return false;
                    return details.level == level;
                })
                .ToList();

            foreach (var banner in _banners)
                banner.SetActive(_eligibleToShow && _active && bannersToDisplay.Any(b => b == banner));
        }

        // private void DisplayBanners()
        // {
        //     var level0Banners = _banners
        //         .Where(banner => banner.GetComponent<PlayerMenuDetails>().level == 0);
        //     foreach (var banner in level0Banners)
        //         banner.SetActive(_eligibleToShow && _active);
        // }

        private IEnumerator CreateActionBanner(PlayerMenuAction action, int index, int length, int level)
        {
            var originalPosition = new Vector3(0, 0, -MenuGap * (length - 1) / 2);

            if (action == PlayerMenuAction.None) yield break;
            var actionName = Enum.GetName(typeof(PlayerMenuAction), action);
            if (actionName == null) yield break;
            var bundleName = $"{actionName.ToLower()}action";

            if (!GameRunner.Instance.AssetMap.ContainsKey(bundleName)) yield break;
            var bannerAsset = GameRunner.Instance.AssetMap[bundleName];

            if (!bannerAsset) yield break;
            var banner = Instantiate(bannerAsset, transform) as GameObject;
            if (!banner) yield break;
            var details = banner.AddComponent<PlayerMenuDetails>();
            if (details) details.level = level;

            var order = length - 1 - index;
            banner.transform.localPosition = originalPosition + new Vector3(0, 0, order * MenuGap);
            banner.transform.localScale = new Vector3(MenuScale, MenuScale, MenuScale);
            banner.GetComponent<SpriteRenderer>().sortingOrder = 999999;
            _banners.Add(banner);

            banner.SetActive(false);
        }
    }
}
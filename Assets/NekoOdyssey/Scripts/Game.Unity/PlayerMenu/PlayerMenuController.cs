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
        public PlayerMenuAction[] availableActions;
        public PlayerMenuSite site;
        public bool autoActive;

        private List<GameObject> _banners = new();
        private bool _eligibleToShow = false;
        private bool _active = false;

        private IDisposable _activeSubscription;
        private IDisposable _currentActionSubscription;

        private void Awake()
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                Debug.Log($">>player_menu_ready<< awake");
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
            for (var i = 0; i < availableActions.Length; i++)
            {
                var action = availableActions[i];
                StartCoroutine(CreateActionBanner(action, i, availableActions.Length));
            }
        }

        private void Start()
        {
            _activeSubscription = GameRunner.Instance.Core.PlayerMenu.OnActive.Subscribe(SetMenuActive);
            _currentActionSubscription = GameRunner.Instance.Core.PlayerMenu.OnChangeAction
                .Subscribe(TriggerCurrentAction);
        }

        private void OnDestroy()
        {
            _activeSubscription.Dispose();
            _currentActionSubscription.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerStay(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _eligibleToShow = true;
            Debug.Log($">>trigger_stay<< {site}");
            GameRunner.Instance.Core.PlayerMenuCandidateManager.Add(new PlayerMenuCandidate()
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
            Debug.Log($">>trigger_exit<<");
            GameRunner.Instance.Core.PlayerMenuCandidateManager.Remove(new PlayerMenuCandidate()
            {
                Site = site
            });
        }

        private void TriggerCurrentAction(PlayerMenuAction currentAction)
        {
            // Debug.Log($">>compare_site<< {GameRunner.Instance.GameCore.PlayerMenu.Site} {site}");
            if (GameRunner.Instance.Core.PlayerMenu.Site != site) return;
            var availableActionList = availableActions.ToList();
            foreach (var action in availableActionList)
            {
                var index = availableActionList.IndexOf(action);
                // Debug.Log($">>index<< {index} {action} {_banners.Count}");
                var banner = _banners[index];
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
            DisplayBanners();
        }

        private void DisplayBanners()
        {
            foreach (var banner in _banners)
            {
                banner.SetActive(_eligibleToShow && _active);
            }
        }

        private IEnumerator CreateActionBanner(PlayerMenuAction action, int index, int length)
        {
            const float scale = 1;
            const float originalGap = 0.4f;
            const float gap = scale * originalGap;
            var originalPosition = new Vector3(0, 0, -gap * (length - 1) / 2);
            Debug.Log($">>banner_01<<");

            if (action == PlayerMenuAction.None) yield break;
            var actionName = Enum.GetName(typeof(PlayerMenuAction), action);
            var bundleName = $"{actionName.ToLower()}action";
            Debug.Log($">>banner_02<<");

            if (!GameRunner.Instance.AssetMap.ContainsKey(bundleName)) yield break;
            var bannerAsset = GameRunner.Instance.AssetMap[bundleName];
            Debug.Log($">>banner_03<<");

            if (bannerAsset == null) yield break;
            var banner = Instantiate(bannerAsset, transform) as GameObject;
            if (banner == null) yield break;
            Debug.Log($">>banner_04<<");

            var order = length - 1 - index;
            banner.transform.localPosition = originalPosition + new Vector3(0, 0, order * gap);
            banner.transform.localScale = new Vector3(scale, scale, scale);
            banner.GetComponent<SpriteRenderer>().sortingOrder = 999999;
            _banners.Add(banner);

            banner.SetActive(false);
        }
    }
}
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Models;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas
{
    public class PhoneSocialNetworkController : MonoBehaviour
    {
        private GameObject _socialFeedCell;

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.GameCore.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            _socialFeedCell = phoneCanvasController.socialFeedCell;
        }

        private void Start()
        {
            GenerateSocialFeedGrid(GameRunner.Instance.GameCore.Player.Phone.SocialNetwork.Feeds);
        }

        private void GenerateSocialFeedGrid(List<SocialFeed> feeds)
        {
            foreach (var feed in feeds)
            {
                Debug.Log($">>feed<< {feed}");
                AddSocialFeedCell(feed);
            }
        }

        private void AddSocialFeedCell(SocialFeed feed)
        {
            var newPostObject = Instantiate(_socialFeedCell, _socialFeedCell.transform.parent);
            newPostObject.SetActive(true);
        }
    }
}
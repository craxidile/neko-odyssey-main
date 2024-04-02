using System.Collections.Generic;
using NekoOdyssey.Scripts.Models;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class SocialNetworkApp
    {
        private static readonly List<SocialFeed> _feeds = new()
        {
            new SocialFeed()
            {
                CatCode = "A02"
            }
        };
        
        public List<SocialFeed> Feeds => _feeds;

        public Subject<List<SocialFeed>> OnChangeFeeds { get; } = new();
            
        public GameObject GameObject { get; set; }

        public void Add(SocialFeed feed)
        {
            Feeds.Insert(0, feed);
            OnChangeFeeds.OnNext(Feeds);
        }
        
        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }
    }
}
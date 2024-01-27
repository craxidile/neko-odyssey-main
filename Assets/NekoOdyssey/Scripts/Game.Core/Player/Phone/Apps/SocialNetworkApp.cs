using System.Collections.Generic;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Models;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class SocialNetworkApp
    {
        public List<SocialFeed> Feeds { get;  } = new();

        public Subject<List<SocialFeed>> OnChangeFeeds = new();
            
        public GameObject GameObject { get; set; }

        public void Add(SocialFeed feed)
        {
            Feeds.Insert(0, feed);
            OnChangeFeeds.OnNext(Feeds);
        }
        
        public void Bind()
        {
            for (var i = 0; i < 2; i++)
            {
                Feeds.Add(new SocialFeed()
                {
                    CatCode = "A02"
                });
            }
            OnChangeFeeds.OnNext(Feeds);
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }
    }
}
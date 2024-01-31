using System.Reflection;
using NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone
{
    public class PlayerPhone
    {
        public PhotoGalleryApp PhotoGallery { get; } = new();
        public SocialNetworkApp SocialNetwork { get; } = new();
        
        public GameObject GameObject { get; set; }

        public void Bind()
        {
            PhotoGallery.Bind();
            SocialNetwork.Bind();
        }

        public void Start()
        {
            PhotoGallery.Start();
            SocialNetwork.Start();
        }

        public void Unbind()
        {
            PhotoGallery.Unbind();
            SocialNetwork.Unbind();
        }
        
    }
}
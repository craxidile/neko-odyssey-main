using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Models;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class PhotoGalleryApp
    {
        public List<PhotoGalleryEntry> Entries { get; } = new();

        public Subject<List<PhotoGalleryEntry>> OnChangeEntries = new();
            
        public GameObject GameObject { get; set; }

        public void Bind()
        {
            for (var i = 0; i < 9; i++)
            {
                Entries.Add(new PhotoGalleryEntry());
            }
            OnChangeEntries.OnNext(Entries);
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }
    }
}
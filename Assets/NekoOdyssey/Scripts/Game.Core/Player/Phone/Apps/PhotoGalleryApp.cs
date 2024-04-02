using System.Collections.Generic;
using NekoOdyssey.Scripts.Models;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Phone.Apps
{
    public class PhotoGalleryApp
    {
        private static readonly List<PhotoGalleryEntry> _entries = new()
        {
            new PhotoGalleryEntry()
            {
                CatCode = "A02"
            }
        };

        public List<PhotoGalleryEntry> Entries => _entries;

        public Subject<List<PhotoGalleryEntry>> OnChangeEntries { get; } = new();

        public GameObject GameObject { get; set; }

        public void Add(PhotoGalleryEntry entry)
        {
            Entries.Add(entry);
            OnChangeEntries.OnNext(Entries);
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
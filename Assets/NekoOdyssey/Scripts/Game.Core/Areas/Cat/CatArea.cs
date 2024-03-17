using System;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Areas.Cat
{
    public class CatArea
    {
        public string Id { get; private set; }
        public Bounds Bounds { get; private set; }
        
        public GameObject GameObject { get; set; }

        public CatArea(Bounds bounds)
        {
            var uuid = Guid.NewGuid(); 
            Id = uuid.ToString();
            Bounds = bounds;
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
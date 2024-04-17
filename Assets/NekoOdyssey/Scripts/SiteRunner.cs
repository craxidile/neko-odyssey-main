using System;
using NekoOdyssey.Scripts.Site.Core;
using NekoOdyssey.Scripts.Site.Unity.Transition;
using UnityEngine;

namespace NekoOdyssey.Scripts
{
    public class SiteRunner : MonoBehaviour
    {
        public static SiteRunner Instance;

        public SiteCore Core { get; } = new();

        public SiteRunner()
        {
            Instance = this;
        }

        private void Awake()
        {
            Core.Bind();
            gameObject.AddComponent<SiteReloadController>();
        }

        private void Start()
        {
            Core.Start();
        }

        private void OnDestroy()
        {
            Core.Unbind();
        }
    }
}
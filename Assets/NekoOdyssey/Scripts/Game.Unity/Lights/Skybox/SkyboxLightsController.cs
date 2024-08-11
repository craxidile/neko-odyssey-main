using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Commons.Models;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Lights.SkyBox
{
    public class SkyboxLightsController : MonoBehaviour
    {
        public GameObject lightS;
        public GameObject lightE;
        public GameObject lightN;
        public GameObject lightW;

        private Dictionary<FacingDirection, GameObject> _lightMap = new();

        private void Awake()
        {
            _lightMap.Add(FacingDirection.South, lightS);
            _lightMap.Add(FacingDirection.East, lightE);
            _lightMap.Add(FacingDirection.North, lightN);
            _lightMap.Add(FacingDirection.West, lightW);
        }

        public void Start()
        {
            SetupLights();
        }

        private void SetupLights()
        {
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            if (currentSite == null) return;

            var lightFacing = currentSite.LightFacingDirection;
            if (lightFacing == FacingDirection.None) return;

            var activeLight = _lightMap[lightFacing];
            if (!activeLight) return;
            
            activeLight.SetActive(true);
            var otherLights = _lightMap.Values.Where(l => l != activeLight);
            foreach (var light in otherLights)
                light.SetActive(false);
        }
    }
}
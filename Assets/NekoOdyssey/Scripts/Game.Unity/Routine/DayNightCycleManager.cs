using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using NekoOdyssey.Scripts.Game.Unity.CustomAttribute;


namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class DayNightCycleManager : MonoBehaviour
    {
        [Range(0f, 24f)]
        public float timeHour = 0;

        [ReadOnlyField]
        [SerializeField]
        float _timeFactor;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
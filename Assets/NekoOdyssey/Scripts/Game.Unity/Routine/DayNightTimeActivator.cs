using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class DayNightTimeActivator : MonoBehaviour
    {
        [SerializeField] string enableTime = "09.00-18.00";
        [SerializeField]
        List<Day> enableDays = new List<Day>() { Day.Mon, Day.Tue, Day.Wed, Day.Thu, Day.Fri, Day.Sat, Day.Sun };


        [SerializeField] List<GameObject> additionTargets = new List<GameObject>();


        public static List<DayNightTimeActivator> DayNightTimeActivators { get; set; } = new List<DayNightTimeActivator>();


        public bool isActivation { get; set; }

        float _delayTime;


        private void Start()
        {
            DayNightTimeActivators.Add(this);

            CheckActivation();
        }
        private void OnDestroy()
        {
            DayNightTimeActivators.Remove(this);
        }

        void CheckActivation()
        {
            if (Time.time < _delayTime) return;
            if (GameRunner.Instance == null || GameRunner.Instance.TimeRoutine == null) return;

            if (GameRunner.Instance.TimeRoutine.inBetweenDayAndTime(enableDays, enableTime))
            {
                SetActive(true);


                _delayTime = Time.time + 1f;
                return;
            }
            else
            {
                SetActive(false);

            }
        }

        void SetActive(bool condition)
        {
            gameObject.SetActive(condition);
            foreach (var target in additionTargets)
            {
                if (target == null) continue;
                target.SetActive(condition);
            }
        }


        public static void UpdateActivator()
        {
            foreach (var dnta in DayNightTimeActivators)
            {
                dnta.CheckActivation();
            }
        }
    }
}
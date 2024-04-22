using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class DayNightTimeActivator : MonoBehaviour
    {
        [SerializeField]
        List<Day> enableDays = new List<Day>() { Day.Mon, Day.Tue, Day.Wed, Day.Thu, Day.Fri, Day.Sat, Day.Sun };
        [SerializeField] string enableTime = "09.00-18.00";


        public bool isActivation { get; set; }

        float _delayTime;


        private void Start()
        {
            Routine.dayNightTimeActivators.Add(this);
            CheckActivation();
        }
        private void OnDestroy()
        {
            Routine.dayNightTimeActivators.Remove(this);
        }

        public void CheckActivation()
        {
            if (Time.time < _delayTime) return;

            if (TimeRoutine.inBetweenDayAndTime(enableDays, enableTime))
            {
                gameObject.SetActive(true);


                _delayTime = Time.time + 1f;
                return;
            }
            else
            {
                gameObject.SetActive(false);

            }
        }
    }
}
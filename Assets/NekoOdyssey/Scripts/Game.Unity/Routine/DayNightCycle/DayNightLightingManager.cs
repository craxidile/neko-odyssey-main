using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

//using NekoOdyssey.Scripts.Game.Unity.AssetBundles;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class DayNightLightingManager
    {
        //[SerializeField] TimeRoutine timeRoutine;

        //List<DayNightDataProfile_Scriptable> dayNightDatas;

        TimeScriptable timeProfile;

        public static DayNightDataProfile_Scriptable currentDayNightProfile;

        float _delayTime;

        bool _initialized = false;
        bool _enabled = true;

        // Start is called before the first frame update
        public void Start()
        {
            //timeProfile = GameRunner.Instance.AssetMap["TimeProfile".ToLower()] as TimeScriptable;
            //timeProfile = GameRunner.Instance.CsvHolder.timeProfile;

            //UnloadAll();

            _delayTime = 0.1f;

            //if (SiteRunner.Instance.Core.Site.Ready)
            //{
            //    currentDayNightProfile = null;
            //    CheckDayNightCondition();
            //    return;
            //}

            //SiteRunner.Instance.Core.Site.OnReady
            //    .Subscribe(_ =>
            //    {
            //        currentDayNightProfile = null;
            //        CheckDayNightCondition();
            //    })
            //    .AddTo(SiteRunner.Instance);



            //SiteRunner.Instance.Core.Site.OnChangeSite
            //    .Subscribe(_ =>
            //    {
            //        currentDayNightProfile = null;
            //        CheckDayNightCondition();
            //    })
            //    .AddTo(SiteRunner.Instance);





            GameRunner.Instance.TimeRoutine.OnChangeDay
                .Subscribe(_ => HandleChangeDay())
                .AddTo(GameRunner.Instance);
        }

        // Update is called once per frame
        public void Update()
        {
            //if (Time.time < _delayTime) return;

            //if (!_initialized)
            //{
            //    Initialized();
            //    return;
            //}

            //if (!_enabled) return;

            //CheckDayNightCondition();
        }

        void Initialized()
        {
            _initialized = true;
            //_delayTime = 0.2f;

            currentDayNightProfile = null;
            //CheckDayNightCondition();


            if (GameRunner.Instance.CsvHolder == null) return;
            if (timeProfile == null)
            {
                timeProfile = GameRunner.Instance.CsvHolder.timeProfile;
                if (timeProfile == null)
                {
                    Debug.Log("time profile is Null");
                    return;
                }
                //UnloadAll();
            }

            _enabled = false;
            foreach (var dnData in timeProfile.dayNightDataProfiles)
            {
                var scene = SceneManager.GetSceneByName(dnData.sceneName);

                if (scene.IsValid())
                {
                    SceneManager.UnloadSceneAsync(scene);
                    _enabled = true;
                }
            }

            if (!_enabled) return;

            //foreach (var dnData in timeProfile.dayNightDataProfiles)
            //{
            //    if (GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(dnData.enableTime))
            //    {
            //        if (currentDayNightProfile != dnData)
            //        {
            //            Debug.Log("time profile #1");
            //            UpdateDayNightProfile(dnData);
            //            _delayTime = Time.time + 1f;
            //        }
            //        return;
            //    }
            //}
        }

        void CheckDayNightCondition()
        {
            //if (timeProfile == null)
            //{
            //    timeProfile = GameRunner.Instance.CsvHolder.timeProfile;
            //    if (timeProfile == null)
            //    {
            //        Debug.Log("time profile is Null");
            //        return;
            //    }
            //    UnloadAll();
            //}

            if (timeProfile == null) return;
            foreach (var dnData in timeProfile.dayNightDataProfiles)
            {
                if (GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(dnData.enableTime))
                {
                    if (currentDayNightProfile != dnData)
                    {
                        Debug.Log("time profile #1");
                        UpdateDayNightProfile(dnData);
                        _delayTime = Time.time + 1f;
                    }
                    return;
                }
            }
        }

        //void UnloadAll()
        //{
        //    foreach (var dnData in timeProfile.dayNightDataProfiles)
        //    {
        //        var scene = SceneManager.GetSceneByName(dnData.sceneName);

        //        if (scene.IsValid())
        //        {
        //            SceneManager.UnloadSceneAsync(scene);
        //        }
        //    }
        //}

        void UpdateDayNightProfile(DayNightDataProfile_Scriptable dnData)
        {
            Debug.Log("time profile #2");
            var previosDayNightProfile = currentDayNightProfile;
            currentDayNightProfile = dnData;

            SceneManager.LoadSceneAsync(dnData.sceneName, LoadSceneMode.Additive).completed += _ =>
            {
                var scene = SceneManager.GetSceneByName(dnData.sceneName);
                SceneManager.SetActiveScene(scene);


                if (previosDayNightProfile != null)
                {
                    //UnloadDayNightData(() =>
                    //{
                    //currentDayNightProfile = dnData;
                    //    LoadDayNightData();
                    //});

                    SceneManager.UnloadSceneAsync(previosDayNightProfile.sceneName);
                }
                else
                {
                    //currentDayNightProfile = dnData;
                    //LoadDayNightData();

                }


                var cam = Camera.main;
                var postProcessVolumn = cam.GetComponent<PostProcessVolume>();

                postProcessVolumn.profile = dnData.postProcessProfile;
            };


        }

        void HandleChangeDay()
        {
            currentDayNightProfile = null;
        }


        //void UnloadDayNightData(Action callback = null)
        //{
        //    SceneManager.UnloadSceneAsync(currentDayNightProfile.sceneName).completed += _ =>
        //    {
        //        callback?.Invoke();
        //    };
        //}

        //void LoadDayNightData(Action callback = null)
        //{
        //    SceneManager.LoadSceneAsync(currentDayNightProfile.sceneName, LoadSceneMode.Additive).completed += _ =>
        //    {
        //        callback?.Invoke();
        //    };
        //}
    }
}
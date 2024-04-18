using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class DayNightLightingManager : MonoBehaviour
{
    //[SerializeField] TimeRoutine timeRoutine;

    [SerializeField] List<DayNightDataProfile_Scriptable> dayNightDatas = new List<DayNightDataProfile_Scriptable>();

    public static DayNightDataProfile_Scriptable currentDayNightProfile;

    float _delayTime;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var dnData in dayNightDatas)
        {
            var scene = SceneManager.GetSceneByName(dnData.sceneName);

            if (scene.IsValid())
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < _delayTime) return;

        foreach (var dnData in dayNightDatas)
        {
            if (TimeRoutine.currentTime.inBetweenTime(dnData.enableTime))
            {
                if (currentDayNightProfile != dnData)
                {
                    updateDayNightProfile(dnData);
                    _delayTime = Time.time + 1f;
                    return;
                }
            }
        }
    }


    void updateDayNightProfile(DayNightDataProfile_Scriptable dnData)
    {
        var previosDayNightProfile = currentDayNightProfile;
        currentDayNightProfile = dnData;

        SceneManager.LoadSceneAsync(dnData.sceneName, LoadSceneMode.Additive).completed += _ =>
        {
            var scene = SceneManager.GetSceneByName(dnData.sceneName);
            SceneManager.SetActiveScene(scene);

        };

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
    }


    void UnloadDayNightData(Action callback = null)
    {
        SceneManager.UnloadSceneAsync(currentDayNightProfile.sceneName).completed += _ =>
        {
            callback?.Invoke();
        };
    }

    void LoadDayNightData(Action callback = null)
    {
        SceneManager.LoadSceneAsync(currentDayNightProfile.sceneName, LoadSceneMode.Additive).completed += _ =>
        {
            callback?.Invoke();
        };
    }
}

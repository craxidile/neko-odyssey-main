using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UniRx;
using NekoOdyssey.Scripts;
using UnityEngine.SceneManagement;

public class DayNightLightingController : MonoBehaviour
{
    [SerializeField] List<DayNightLightingProfile> LightingProfiles = new List<DayNightLightingProfile>();

    //TimeScriptable timeProfile;

    DayNightLightingProfile currentDayNightProfile;

    float _delayTime;

    bool _needSetActive;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var lightProfile in LightingProfiles)
        {
            lightProfile.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < _delayTime) return;

        if (GameRunner.Instance == null || GameRunner.Instance.TimeRoutine == null)
        {
            UpdateDayNightProfile(LightingProfiles[0]);
            return;
        }
        //if (timeProfile == null)
        //{
        //    timeProfile = GameRunner.Instance.CsvHolder.timeProfile;
        //    if (timeProfile == null)
        //    {
        //        Debug.Log("time profile is Null");
        //        return;
        //    }
        //}

        //bool alreadySet = false;
        DayNightLightingProfile targetProfile = currentDayNightProfile;
        foreach (var lightProfile in LightingProfiles)
        {

            //if (alreadySet)
            //{
            //    continue;
            //}

            if (GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(lightProfile.enableTime))
            {
                //if (currentDayNightProfile != lightProfile)
                //{
                Debug.Log("time profile #1");
                targetProfile = lightProfile;
                //UpdateDayNightProfile(lightProfile);
                //_delayTime = Time.time + 1f;
                //}

                //alreadySet = true;
            }
            else
            {
                lightProfile.gameObject.SetActive(false);
            }
        }
        if (currentDayNightProfile != targetProfile)
        {
            UpdateDayNightProfile(targetProfile);
            _delayTime = Time.time + 1f;
        }


        if (NekoOdyssey.Scripts.Site.Unity.Transition.SiteTransitionController.isSetActiveScene && _needSetActive)
        {
            _needSetActive = false;

            var scene = SceneManager.GetSceneByName("SkyBox");
            SceneManager.SetActiveScene(scene);
        }
    }

    void UpdateDayNightProfile(DayNightLightingProfile lightProfile)
    {
        Debug.Log($"time profile #2 {lightProfile.gameObject.name}");

        currentDayNightProfile = lightProfile;


        var scene = SceneManager.GetSceneByName("SkyBox");
        SceneManager.SetActiveScene(scene);
        _needSetActive = true;


        lightProfile.gameObject.SetActive(true);


        var cam = Camera.main;
        var postProcessVolumn = cam.GetComponent<PostProcessVolume>();
        postProcessVolumn.profile = lightProfile.postProcessProfile;


        RenderSettings.skybox = lightProfile.skyboxMaterial;

    }
}
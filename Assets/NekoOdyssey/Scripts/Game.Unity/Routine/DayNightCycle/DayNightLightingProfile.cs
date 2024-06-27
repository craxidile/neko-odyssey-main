using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DayNightLightingProfile : MonoBehaviour
{
    [Tooltip("Example : 09.35-17.55")]
    public string enableTime;

    public Material skyboxMaterial;
    public PostProcessProfile postProcessProfile;
}

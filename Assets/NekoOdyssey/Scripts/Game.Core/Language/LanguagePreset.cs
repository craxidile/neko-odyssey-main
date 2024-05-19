using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "LanguagePreset", menuName = "Scene Preset/Language preset", order = 1)]
public class LanguagePreset : ScriptableObject
{
    public List<LanguageComponent> Datas;
}

[System.Serializable]
public  class LanguageComponent
{
    [Header("General")]
    public languageType language;
    public Sprite languageNameImage;
    public TMP_FontAsset uiTMPFont;
    //public TMP_FontAsset characterNameTMPFont;
}

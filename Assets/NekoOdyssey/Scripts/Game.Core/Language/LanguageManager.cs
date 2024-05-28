using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum languageType
{
    EN,
    TH,
    JP,
    S_CN,
    T_CN,
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public languageType language = languageType.EN;
    public static languageType globalLanguage = languageType.EN;

    public void updateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} / {globalLanguage}");
        Instance = this;
        language = globalLanguage;
    }


}
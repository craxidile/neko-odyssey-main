using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class LanguagePresetProvider : MonoBehaviour
{
    [SerializeField] public LanguagePreset _languagePreset;

    public LanguageComponent GetLanguageComponent()
    {
        var selectedlanguagePreset = _languagePreset.Datas.Where(l => l.language == LanguageManager.globalLanguage).First();
        return selectedlanguagePreset;
    }

    public static LanguagePresetProvider Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }
}

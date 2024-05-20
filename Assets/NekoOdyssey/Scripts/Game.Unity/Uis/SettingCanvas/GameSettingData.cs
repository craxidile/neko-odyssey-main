using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettingData
{
    public int ContinueSaveSlot;
    public int LanguageIndex;
    public float MasterVolume, BgmVolume, EffectVolume;

    public GameSettingData()
    {
        ContinueSaveSlot = 0;
        LanguageIndex = 0;
        MasterVolume = 0.5f;
        BgmVolume = 1f;
        EffectVolume = 1f;
    }
}


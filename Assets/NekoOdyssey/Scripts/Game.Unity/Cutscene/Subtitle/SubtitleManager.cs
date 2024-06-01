using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleData
{
    public string SubtitleSentance;

    public string[] GetSubtitleSentanceSeperateLine()
    {
        return SubtitleSentance.Split(('\n')).ToArray();
    }
}

public class SubtitleManager : MonoBehaviour
{
    //languege 
    int languageColumnIndex = 1;
    public languageType language = languageType.EN;
    public static languageType globalLanguage = LanguageManager.globalLanguage;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} to {globalLanguage}");
        language = globalLanguage;
    }

    [SerializeField] TextAsset SubtitleAsset;

    static Dictionary<string, SubtitleData> AllSubtitleData = new Dictionary<string, SubtitleData>();

    public static SubtitleData GetSubtitle(string lineIndexID)
    {
        return AllSubtitleData[lineIndexID];
    }

    private void Awake()
    {
        UpdateGlobalLanguage();
        LoadSubtitleCSV();
    }

    public void LoadSubtitleCSV()
    {
        string[] data = SubtitleAsset.text.Split(('\n')).ToArray();
        CheckColumnIndex(data[0]);

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split((',')).ToArray();

            SubtitleData newSubtitleData = new SubtitleData();

            string subtitle = row[languageColumnIndex];
            subtitle = subtitle.Replace(';', ',');
            subtitle = subtitle.Replace('_', '\n');
            newSubtitleData.SubtitleSentance = subtitle;

            if (!AllSubtitleData.ContainsKey(row[0]))
            {
                AllSubtitleData.Add(row[0], newSubtitleData);
            }
        }

    }
    void CheckColumnIndex(string fristColumn)
    {
        var row = fristColumn.Split((',')).ToArray();

        for (int i = 0; i < row.Length; i++)
        {

            if (row[i].ToLower() == language.ToString().ToLower())
            {
                languageColumnIndex = i;
                Debug.Log($"check language [{row[i]}] (column {i})");
            }
        }
    }

}

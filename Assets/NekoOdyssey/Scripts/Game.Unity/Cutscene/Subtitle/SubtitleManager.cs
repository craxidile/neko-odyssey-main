using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public enum languageTypeSubtitle
{
    TH,
    EN,
    JP,
    S_CN,
    T_CN,
}
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
    int languageColumnIndex = 1;

    //languege  
    public languageTypeSubtitle language = languageTypeSubtitle.EN;
    public static languageTypeSubtitle globalLanguage = languageTypeSubtitle.EN;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} / {globalLanguage}");
        //language = globalLanguage;
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
            newSubtitleData.SubtitleSentance = subtitle;

            AllSubtitleData.Add(row[0], newSubtitleData);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class LoadUiLanguageFromCSV : MonoBehaviour
{
    [SerializeField] TextAsset uiCSV;

    static Dictionary<string, string> UiCompareLanguage = new Dictionary<string, string>();
    int languageColumnIndex = 0;


    public static void GetUiLanguage(ref TextMeshProUGUI text)
    {

        //var selectedlanguagePreset = LanguagePresetProvider.Instance.GetLanguageComponent();
        //text.font = selectedlanguagePreset.uiTMPFont;

        if (!UiCompareLanguage.ContainsKey(text.text)) return;

        var newText = UiCompareLanguage[text.text];
        //Debug.Log($"language ui text {LanguageManager.globalLanguage} : {text.text} -> {newText}");
        text.text = newText;

    }

    public static string GetUiLanguageText(string refText)
    {
        if (!UiCompareLanguage.ContainsKey(refText)) return refText;

        return UiCompareLanguage[refText];
    }


    void Start()
    {

        UiCompareLanguage = loadCSV();
    }
    public void ReloadUiLanguage()
    {
        UiCompareLanguage = loadCSV();
    }


    Dictionary<string, string> loadCSV()
    {
        var newDataDictionary = new Dictionary<string, string>();
        string[] data = uiCSV.text.Trim().Split(('\n')).ToArray();
        CheckColumnLanguage(data[0]);


        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split((',')).ToArray();

            string newText = row[languageColumnIndex];

            newText = newText.Replace(';', ',');
            newText = newText.Replace("[n]", "\n");
            newText = newText.Replace("[N]", "\n");

            newDataDictionary.Add(row[0], newText);
        }

        return newDataDictionary;
    }
    void CheckColumnLanguage(string fristColumn)
    {
        var row = fristColumn.Split((',')).ToArray();

        for (int j = 0; j < row.Length; j++)
        {

            if (row[j].ToLower() == LanguageManager.globalLanguage.ToString().ToLower())
            {
                languageColumnIndex = j;
            }
        }
    }
}

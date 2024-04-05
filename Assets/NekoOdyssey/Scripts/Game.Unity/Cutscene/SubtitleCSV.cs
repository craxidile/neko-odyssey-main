using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
public enum languageType
{
    English,
    Japanese,
    Chinese,
    Thai
}
public class DialogueData
{
    public string DialogueSentance;

    public string[] GetDialogueSentanceSeperateLine()
    {
        return DialogueSentance.Split(('\n')).ToArray();
    }
}

public class SubtitleCSV : MonoBehaviour
{

    private PlayableDirector director;
    //languege  
    public languageType language = languageType.English;
    public static languageType globalLanguage = languageType.English;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} / {globalLanguage}");
        language = globalLanguage;
    }

    // Dialogue
    [SerializeField] TextAsset DialogueAsset;

    int languageColumnIndex = 1;

    static Dictionary<string, DialogueData> AllDialogueData = new Dictionary<string, DialogueData>();

    public static DialogueData GetDialogue(string lineIndexID)
    {
        return AllDialogueData[lineIndexID];
    }

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        UpdateGlobalLanguage();
        LoadDialogueCSV();
    }


    private void Update()
    {
        if (director.state == PlayState.Paused && Input.anyKeyDown)
        {
            director.Play();
            Debug.Log($">>behavior<< play after get key down");
        }
    }

    public void LoadDialogueCSV()
    {
        string[] data = DialogueAsset.text.Split(('\n')).ToArray();
        CheckColumnIndex(data[0]);



        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split((',')).ToArray();

            DialogueData newDialogueData = new DialogueData();

            string dialogue = row[languageColumnIndex];

            dialogue = dialogue.Replace(';', ',');
            dialogue = dialogue.Replace("[n]", "\n");
            dialogue = dialogue.Replace("[N]", "\n");
            newDialogueData.DialogueSentance = dialogue;

            AllDialogueData.Add(row[0], newDialogueData);
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

    public static string ReadCSV(TextAsset textAsset, string rowIndexId)
    {
        string[] data = textAsset.text.Split(('\n')).ToArray();

        var firstrow = data[0].Split((',')).ToArray();

        int languageColumnIndex = 0;

        for (int i = 0; i < firstrow.Length; i++)
        {
            if (firstrow[i].ToLower() == "Th")
            {
                languageColumnIndex = i;
            }


        }

        for (int i = 1; i < data.Length; i++)
        {
            var row = data[i].Split((',')).ToArray();

            if (row[0] == rowIndexId)
            {
                return row[languageColumnIndex];
            }
        }

        return "";
    }
    public static void FunctionA(GameObject Position)
    {
        Debug.Log("function A gameOject :" + Position.ToString());
    }

}

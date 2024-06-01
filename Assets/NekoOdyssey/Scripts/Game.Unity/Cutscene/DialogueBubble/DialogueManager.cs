using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueData
{
    public string DialogueSentance;

    public string[] GetDialogueSentanceSeperateLine()
    {
        return DialogueSentance.Split(('\n')).ToArray();
    }
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [HideInInspector]
    public PlayableDirector director;
    public DialogCanvasController canvasController;
    public bool endBubble;

    //languege
    int languageColumnIndex = 1;
        public languageType language = languageType.EN;
    public static languageType globalLanguage = LanguageManager.globalLanguage;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} to {globalLanguage}");
        language = globalLanguage;
    }

    // Dialogue
    [SerializeField] TextAsset DialogueAsset;

    
    static Dictionary<string, DialogueData> AllDialogueData = new Dictionary<string, DialogueData>();

    public static DialogueData GetDialogue(string lineIndexID)
    {
        return AllDialogueData[lineIndexID];
    }
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        canvasController.SetOpened(false);
        UpdateGlobalLanguage();
        LoadDialogueCSV();
    }

    private void Update()
    {
        if (Input.anyKeyDown && !endBubble)
        {
            if (canvasController.lastLineId)
            {
                endBubble = true;
                canvasController.endDialogue = true;
            }
            else
            {
                canvasController.goNextLineId = true;
            }
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
            dialogue = dialogue.Replace('_', '\n');
            newDialogueData.DialogueSentance = dialogue;

            if (!AllDialogueData.ContainsKey(row[0])) 
            {
                AllDialogueData.Add(row[0], newDialogueData);
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


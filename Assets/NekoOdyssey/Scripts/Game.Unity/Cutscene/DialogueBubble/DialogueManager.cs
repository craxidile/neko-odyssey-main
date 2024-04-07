using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public enum languageTypeDialogue
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

public class DialogueManager : MonoBehaviour
{
    int languageColumnIndex = 1;
    public PlayableDirector director;
    //languege  
    public languageTypeDialogue language = languageTypeDialogue.English;
    public static languageTypeDialogue globalLanguage = languageTypeDialogue.English;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} / {globalLanguage}");
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
        UpdateGlobalLanguage();
        LoadDialogueCSV();
    }


    private void Update()
    {
        if (director.state == PlayState.Paused && Input.anyKeyDown)
        {
            director.Play();
            Debug.Log($">>behavior<< play after get key down");
            if (IsEndDialogue)
            {
                EndDialogue();
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

    public static void NextDialogue(GameObject posRef)
    {
        Debug.Log("function A gameOject :" + posRef);

        //ChatBalloonManager.instance.ShowChatBalloon(posRef.transform, "Text1");
    }
    public static void StartDialogue()
    {
        Debug.Log("Start Dialogue");

        
    }
    public static void EndDialogue()
    {
        Debug.Log("End Dialogue");

        //ChatBalloonManager.instance.HideChatBalloon();
    }

    public static bool IsEndDialogue { get; set; }

}


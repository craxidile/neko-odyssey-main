using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using DayOfWeek = NekoOdyssey.Scripts.Database.Commons.Models.DayOfWeek;
using NekoOdyssey.Scripts;
public enum DatabaseDialogueType
{
    CMS,
    CSV,
}
public class DialogueData
{
    public string DialogueSentance;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [Header("Type of data")]
    public DatabaseDialogueType data = DatabaseDialogueType.CMS;

    [Header("CMS")]
    public string npcDialogCode;

    [Header("CSV")]
    [SerializeField] TextAsset DialogueAsset;
    int languageColumnIndex = 1;
    public languageType language = languageType.EN;

    [HideInInspector] public PlayableDirector director;
    [HideInInspector] public DialogCanvasController canvasController;
    [HideInInspector] public bool endBubble;
    //languege


    // Dialogue


    static Dictionary<string, DialogueData> AllDialogueData = new Dictionary<string, DialogueData>();

    public static DialogueData GetDialogue(string lineIndexID)
    {
        return AllDialogueData[lineIndexID];
    }
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        canvasController = FindAnyObjectByType<DialogCanvasController>();
        canvasController.SetOpened(false);
    }
    private void Start()
    {
        if (data == DatabaseDialogueType.CSV)
        {
            LoadDialogueCSV();
        }
        else if (data == DatabaseDialogueType.CMS)
        {
            if (!string.IsNullOrEmpty(npcDialogCode))
            {
                new DialogueManager().LoadDialogueCMS(npcDialogCode);
            }
            else
            {
                Debug.LogWarning($">>npc_dialog_code<< Not Found ");
            }
        }
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
    public void LoadDialogueCMS(string dialogCode)
    {
        var questGroupsMasterData = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData;
        var dialogMasterData = GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData;
        if (questGroupsMasterData.Ready)
        {
            ExecuteDialog(dialogMasterData.Dialogs.FirstOrDefault(d => d.Code == dialogCode));

        }
        else
        {
            questGroupsMasterData.OnReady
                .Subscribe(_ =>
                {
                    ExecuteDialog(dialogMasterData.Dialogs.FirstOrDefault(d => d.Code == dialogCode));
                })
                .AddTo(GameRunner.Instance);
        }

    }

    private void ExecuteDialog(Dialog dialog)
    {
        Debug.Log($">>test_npc<< >>dialog<< {dialog.Code}");

        IDialogNextEntity next;
        next = dialog.NextEntity;
        if (next is SubDialog)
        {
            ExecuteSubDialog(next as SubDialog);
        }
    }

    private void ExecuteSubDialog(SubDialog subDialog)
    {
        Debug.Log($">>sub_dialog<< {subDialog.Id}");
        Debug.Log($">>locale<< {GameRunner.Instance.Core.Settings.Locale}");

        foreach (var line in subDialog.Lines)
        {
            DialogueData newDialogueData = new DialogueData();

            Debug.Log($">>dialog_npc_cutscene<< >>line<< {line.Actor} {line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale)}");
            newDialogueData.DialogueSentance = line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale);

            if (!AllDialogueData.ContainsKey(line.LocalizedText.Original))
            {
                AllDialogueData.Add(line.LocalizedText.Original, newDialogueData);
            }
        }

        var childFlag = subDialog.DialogChildFlag;
        switch (childFlag)
        {
            case DialogChildFlag.End:
                EndDialog();
                break;
            case DialogChildFlag.Cancel:
                CancelDialog();
                break;
            default:
                return;
        }
    }
    private void EndDialog()
    {
        Debug.Log($">>test_npc<< >>end<<");
    }

    private void CancelDialog()
    {
        Debug.Log($">>test_npc<< >>cancel<<");
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


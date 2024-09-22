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

public class SubtitleData
{
    public string SubtitleSentance;
}

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager instance;
    public string dialogCode;
    //languege 
    int languageColumnIndex = 1;
    public languageType language = languageType.EN;
    public static languageType globalLanguage = LanguageManager.globalLanguage;

    public void UpdateGlobalLanguage()
    {
        Debug.Log($"ChangeLanguage : {language} to {globalLanguage}");
        language = globalLanguage;
    }

    public bool useCSV = true;
    [SerializeField] TextAsset SubtitleAsset;

    static Dictionary<string, SubtitleData> AllSubtitleData = new Dictionary<string, SubtitleData>();

    public static SubtitleData GetSubtitle(string lineIndexID)
    {
        return AllSubtitleData[lineIndexID];
    }

    private void Awake()
    {
        UpdateGlobalLanguage();
    }
    private void Start()
    {
        if (useCSV)
        {
            LoadSubtitleCSV();
        }
        else
        {
            new SubtitleManager().LoadSubtitleCMS();
        }
    }

    public void LoadSubtitleCMS()
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

        var indexArray = 1;
        foreach (var line in subDialog.Lines)
        {
            SubtitleData newSubtitleData = new SubtitleData();

            Debug.Log($">>dialog_npc_cutscene<< >>line<< {line.Actor} {line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale)}");
            newSubtitleData.SubtitleSentance = line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale);

            if (!AllSubtitleData.ContainsKey(indexArray.ToString("D3")))
            {
                AllSubtitleData.Add(indexArray.ToString("D3"), newSubtitleData);
            }
            indexArray++;
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

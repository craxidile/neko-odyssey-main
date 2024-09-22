using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSVHolderScriptable", menuName = "ScriptableObjects/CSVHolderScriptable")]
public class CSVHolderScriptable : ScriptableObject
{
    public TimeScriptable timeProfile;

    [Header("Npc Routine")]
    public TextAsset[] routinesCSV;

    [Space]
    [Header("All Quest Line")]
    public TextAsset questRelationshipCSV;
    public TextAsset[] allQuestsCSV;

    [Space]
    [Header("All Quest Dialogue")]
    public TextAsset[] allQuestDialoguesCSV;

    [Space]
    public bool playFinishDemoVideo;
}

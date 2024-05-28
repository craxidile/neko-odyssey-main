using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Globalization;

namespace DataPersistence
{

    [System.Serializable]
    public class GameData
    {
        public float GameVersion;
        public int Money;
        public int Money_Used;
        //public PlayerData playerData;
        //public List<CharacterData> CharacterDatas;

        //public List<string> UnlockedStages;
        //public int LastestChapter;
        //public string LastestPlayedMapCode;
        public int PlayedTime;
        public string SavedDate;

        public List<string> UnlockedItem;
        public List<InventoryItemData> InventoryItem;
        public int ItemInventoryIdCounting;

        public List<string> UnlockedStoryData;
        //public List<string> UnlockedCharacterSpecialAttack;

        public List<string> RewardedStages;

        public FirstTimeData firstTimeData;

        //set default constructor
        public GameData(float gameVersion)
        {
            GameVersion = gameVersion;
            Money = Money_Used = 0;
            //CharacterDatas = new List<CharacterData>();
            //CharacterDatas.Add(new CharacterData("M01"));
            //CharacterDatas.Add(new CharacterData("M02"));
            //playerData = new PlayerData();
            //playerData.TeamsUP.Add(new PlayerData.TeamUp());
            //playerData.TeamsUP[0].TeamUpCharacters = new PlayerData.TeamUpCharacter[5];
            //playerData.TeamsUP[0] = "M01";
            //playerData.TeamsUP[1] = "M02";
            //playerData.TeamsUP[2] = "";
            //playerData.TeamsUP[3] = "";
            //playerData.TeamsUP[4] = "";


            //UnlockedStages = new List<string>();
            //LastestChapter = 0;
            //LastestPlayedMapCode = "";
            PlayedTime = 0;

            SavedDate = DateTime.Now.ToString("dd MMM yyyy | hh:mmtt", CultureInfo.InvariantCulture);

            UnlockedItem = new List<string>();
            InventoryItem = new List<InventoryItemData>();
            ItemInventoryIdCounting = 1;

            UnlockedStoryData = new List<string>();
            //UnlockedCharacterSpecialAttack = new List<string>();

            RewardedStages = new List<string>();

            firstTimeData = new FirstTimeData();
        }
    }



    //[Serializable]
    //public class PlayerData
    //{
    //    public List<TeamUp> TeamsUP = new List<TeamUp>();
    //    public string[] TeamsUP = new string[5];

    //    public bool IsTeamUPEmpty => !TeamsUP.Any(character => character != "");

    //    [System.Serializable]
    //    public class TeamUp
    //    {
    //        public TeamUpCharacter[] TeamUpCharacters = new TeamUpCharacter[5];
    //    }
    //    [System.Serializable]
    //    public class TeamUpCharacter
    //    {
    //        public string CharacterAchetype = "";
    //    }
    //}


    //[Serializable]
    //public class CharacterData
    //{
    //    public string Archetype;
    //    public float Exp;
    //    public float TempolaryExp;
    //    public int Level;

    //    public int UpgradeLevel_HP, UpgradeLevel_ATK, UpgradeLevel_DEF, UpgradeLevel_Acc;
    //    public int[] ExAttackLevel;
    //    public int[] ItemEquiped;

    //    public CharacterData(string archetype)
    //    {
    //        Archetype = archetype;
    //        Exp = TempolaryExp = 0;
    //        Level = 1;

    //        UpgradeLevel_HP = 0;
    //        UpgradeLevel_ATK = 0;
    //        UpgradeLevel_DEF = 0;
    //        UpgradeLevel_Acc = 0;

    //        ExAttackLevel = new int[3] { 1, 1, 1 };

    //        ItemEquiped = new int[3] { 0, 0, 0 };
    //    }
    //    public int[] GetUpgradedStatus(int hp, int atk, int def, int acc)
    //    {
    //        float upgradeMultiplyValue = 0.1f;
    //        if (DevTools_GameSettingPreset.instance != null)
    //        {
    //            upgradeMultiplyValue = DevTools_GameSettingPreset.instance.StatusUpgrade_ValuePerLevel;
    //        }

    //        return new int[]
    //        {
    //            (int)(hp * (1+(((float)UpgradeLevel_HP)*upgradeMultiplyValue))) ,
    //            (int)(atk * (1+(((float)UpgradeLevel_ATK)*upgradeMultiplyValue))) ,
    //            (int)(def * (1+(((float)UpgradeLevel_DEF)*upgradeMultiplyValue)) ),
    //            (int)(acc * (1+(((float)UpgradeLevel_Acc)*upgradeMultiplyValue)))
    //        };
    //    }
    //}

    [Serializable]
    public class InventoryItemData
    {
        public int ItemInventoryId;
        public string ItemCode;
        public string EquipedCharacter = "";

        public InventoryItemData(string itemCode, int itemId)
        {
            ItemCode = itemCode;
            ItemInventoryId = itemId;
        }
    }


    [Serializable]
    public class FirstTimeData
    {
        public bool NewItemShopUnlocked, NewFightClubUnlocked, NewAllyUnlocked;
        public List<string> TutorialListCode;


        public FirstTimeData()
        {
            NewItemShopUnlocked = false;
            NewFightClubUnlocked = false;
            NewAllyUnlocked = false;
            TutorialListCode = new List<string>();
        }
    }
}

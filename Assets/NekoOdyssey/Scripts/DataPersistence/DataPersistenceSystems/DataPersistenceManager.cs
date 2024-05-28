using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Linq;

namespace DataPersistence
{
    //C:\Users\User\AppData\LocalLow\Secret Chracter\Neko Odyssey

    public class DataPersistenceManager : MonoBehaviour
    {
        public const int AutoSaveSlotIndex = 0;
        public const int TempSaveSlotIndex = 999999;

        public static bool GameReadyToSave { get; set; } = false;

        [Header("File Stroge Config")] [SerializeField]
        string fileName;

        [SerializeField] string settingFileName;
        [SerializeField] bool useEncryption;

        [SerializeField] float gameVersion;

        GameData gameData;

        GameSettingData settingData;
        //List<IDataPersistence> saveLoadDataObjects;

        FileDataHandler dataHandler;

        public int currentSaveSlot { get; set; } = -1;


        public static DataPersistenceManager instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            //DataCharacter.SetUpDataPersistence(ref gameData);

            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption, gameVersion);
        }

        private void Start()
        {
        }

        public void NewGame()
        {
            this.gameData = new GameData(gameVersion);
        }

        public void LoadGame(int saveSlot)
        {
            gameData = dataHandler.LoadGame(saveSlot);

            if (gameData == null)
            {
                Debug.Log("Load data not found create new Save");
                NewGame();
            }

            var saveLoadDataObjects = FindAllSaveLoadDataObjects();
            foreach (var saveLoadDataObj in saveLoadDataObjects)
            {
                saveLoadDataObj.LoadData(gameData);
            }
        }

        public void LoadGame_CurrentSlot() => LoadGame(currentSaveSlot);

        public void LoadGame_andSetSlotPointer(int saveSlot)
        {
            currentSaveSlot = saveSlot;
            LoadGame(saveSlot);
        }

        public void LoadGameToTempSlot(int saveSlot)
        {
            currentSaveSlot = TempSaveSlotIndex;
            LoadGame(saveSlot);
        }

        public void SaveGame(int saveSlot)
        {
            var saveLoadDataObjects = FindAllSaveLoadDataObjects();
            foreach (var saveLoadDataObj in saveLoadDataObjects)
            {
                saveLoadDataObj.SaveData(ref gameData);
            }

            gameData.SavedDate = DateTime.Now.ToString("dd MMM yyyy | hh:mmtt", CultureInfo.InvariantCulture);
            dataHandler.SaveGame(saveSlot, gameData);
        }

        public void SaveGame(int saveSlot, GameData newGameData)
        {
            var saveLoadDataObjects = FindAllSaveLoadDataObjects();
            foreach (var saveLoadDataObj in saveLoadDataObjects)
            {
                saveLoadDataObj.SaveData(ref newGameData);
            }

            newGameData.SavedDate = DateTime.Now.ToString("dd MMM yyyy | hh:mmtt", CultureInfo.InvariantCulture);
            dataHandler.SaveGame(saveSlot, newGameData);
        }

        public void AutoSaveGame()
        {
            Debug.Log($">>game_ready_to_save<< {GameReadyToSave}");
            if (!GameReadyToSave || currentSaveSlot == -1) return;
            GameReadyToSave = false;
            var saveLoadDataObjects = FindAllSaveLoadDataObjects();
            foreach (var saveLoadDataObj in saveLoadDataObjects)
                saveLoadDataObj.SaveData(ref gameData);

            gameData.SavedDate = DateTime.Now.ToString("dd MMM yyyy | hh:mmtt", CultureInfo.InvariantCulture);
            dataHandler.SaveGame(AutoSaveSlotIndex, gameData);
        }

        public void SaveGame_CurrentSlot()
        {
            if (currentSaveSlot == -1) return;
            SaveGame(currentSaveSlot);
        }

        public void DeleteGame(int saveSlot)
        {
            dataHandler.DeleteGame(saveSlot);
        }

        List<IDataPersistence> FindAllSaveLoadDataObjects()
        {
            var saveloadObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

            return new List<IDataPersistence>(saveloadObjects);
        }

        public bool CheckSaveSlot(int saveSlot)
        {
            var newGameData = dataHandler.LoadGame(saveSlot);
            if (newGameData != null)
            {
                return true;
            }

            return false;
        }


        public GameSettingData GetSettingData()
        {
            if (settingData == null)
            {
                LoadSetting();
            }

            return settingData;
        }

        public void LoadSetting()
        {
            Debug.Log("load setting 1");
            Debug.Log(settingFileName);
            Debug.Log(dataHandler);
            var fileData = dataHandler.LoadSetting(settingFileName);
            Debug.Log("load setting 2");
            if (fileData == null)
            {
                Debug.Log("Load game setting data not found create new SettingData");
                this.settingData = new GameSettingData();
            }
            else
            {
                Debug.Log("load setting 3");
                settingData = fileData;
            }

            Debug.Log("load setting 4");
            Debug.Log("settingData " + settingData.LanguageIndex);
            Debug.Log("settingData " + settingData.MasterVolume);
        }

        public void SaveSetting()
        {
            dataHandler.Save(settingFileName, settingData);
        }

        public GameData GetGameData() => gameData;
        public GameData GetGameData(int saveSlot) => dataHandler.LoadGame(saveSlot);


        public void AddMoney(int value)
        {
            gameData.Money += value;
        }

        public void UsedMoney(int usedMoney)
        {
            gameData.Money -= usedMoney;
            gameData.Money_Used += usedMoney;


        }

        //public bool UnlockNewStages(string stagesCode)
        //{
        //    if (!gameData.UnlockedStages.Any(stage => stage == stagesCode))
        //    {
        //        gameData.UnlockedStages.Add(stagesCode);
        //        gameData.firstTimeData.NewFightClubUnlocked = true;
        //        return true;
        //    }

        //    return false;
        //}

        //public void UnlockNextChapter()
        //{
        //    gameData.LastestChapter += 1;
        //    Debug.Log($"UnlockNextChapter {gameData.LastestChapter - 1} -> {gameData.LastestChapter}");
        //}

        //public bool UnlockNewItem(string ItemCode)
        //{
        //    if (!gameData.UnlockedItem.Contains(ItemCode))
        //    {
        //        gameData.UnlockedItem.Add(ItemCode);
        //        gameData.firstTimeData.NewItemShopUnlocked = true;
        //        return true;
        //    }

        //    return false;
        //}

        //public void AddNewInventoryItem(string ItemCode)
        //{
        //    gameData.InventoryItem.Add(new InventoryItemData(ItemCode, gameData.ItemInventoryIdCounting++));
        //}

        //public void DeleteInventoryItem(InventoryItemData targetItem)
        //{
        //    if (!gameData.InventoryItem.Contains(targetItem))
        //        Debug.LogError($"DeleteInventoryItem Failed : ({targetItem.ItemInventoryId}){targetItem.ItemCode}");

        //    gameData.InventoryItem.Remove(targetItem);

        //    if (!string.IsNullOrEmpty(targetItem.EquipedCharacter))
        //    {
        //        var equipedCharacterData = DataCharacter.GetCharacter(targetItem.EquipedCharacter);
        //        for (int i = 0; i < equipedCharacterData.ItemEquiped.Length; i++)
        //        {
        //            if (equipedCharacterData.ItemEquiped[i] == targetItem.ItemInventoryId)
        //            {
        //                equipedCharacterData.ItemEquiped[i] = 0;
        //                break;
        //            }
        //        }
        //    }
        //}


        //public void UnlockStoryData(string key)
        //{
        //    if (!gameData.UnlockedStoryData.Contains(key))
        //    {
        //        gameData.UnlockedStoryData.Add(key);
        //    }
        //}

        //public void RemoveStoryData(string key)
        //{
        //    if (gameData.UnlockedStoryData.Contains(key))
        //    {
        //        gameData.UnlockedStoryData.Remove(key);
        //    }
        ////}

        //public bool CheckStoryData(string key)
        //{
        //    return gameData.UnlockedStoryData.Contains(key);
        //}

        //public void UnlockCharacterSpecialAttack(string key)
        //{
        //    if (!gameData.UnlockedCharacterSpecialAttack.Contains(key))
        //    {
        //        gameData.UnlockedCharacterSpecialAttack.Add(key);
        //    }
        //}


        //public void AddRewardedStage(string stageCode)
        //{
        //    if (!gameData.RewardedStages.Contains(stageCode))
        //    {
        //        gameData.RewardedStages.Add(stageCode);
        //    }
        //}

        //public bool CheckRewardedStage(string stageCode)
        //{
        //    return gameData.RewardedStages.Contains(stageCode);
        //}


        //public bool TryAddTutorialCode(string key)
        //{
        //    if (!string.IsNullOrEmpty(key))
        //    {
        //        if (gameData?.firstTimeData.TutorialListCode.Contains(key) == false)
        //        {
        //            gameData.firstTimeData.TutorialListCode.Add(key);

        //            return true;
        //        }
        //    }

        //    return false;
        //}


        //#if UNITY_EDITOR
        //        [Button(ButtonSizes.Large)]
        //        private void OpenSaveFileFolder()
        //        {
        //            Debug.Log("Unity Editor");
        //            // Define the path to your external folder
        //            var folderPath = Application.persistentDataPath;

        //            // Open the folder in the file explorer
        //            UnityEditor.EditorUtility.RevealInFinder(folderPath);
        //        }
        //#endif
    }
}
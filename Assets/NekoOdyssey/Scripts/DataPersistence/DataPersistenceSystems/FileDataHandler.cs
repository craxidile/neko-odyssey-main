using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace DataPersistence
{
    public class FileDataHandler
    {
        string dataDirPath = "";
        string dataFileName = "";
        bool useEncryption = false;
        readonly string encryptionCodeWord = "NekoOdyssey";
        float gameVersion = 0;

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption, float gameVersion)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
            this.gameVersion = gameVersion;
        }

        public GameData LoadGame(int saveSlot)
        {
            var gameData = Load<GameData>($"{dataFileName}{saveSlot}");

            //var saveVersion = gameData.GameVersion;
            //if (saveVersion == gameVersion)
            //{
            //    return gameData;

            //}
            return gameData;
            //return null;
        }

        public GameSettingData LoadSetting(string settingFileName) => Load<GameSettingData>(settingFileName);

        public T Load<T>(string fileName)
        {
            string fullPath = Path.Combine(dataDirPath, fileName);
            T loadedData = default(T);
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }

                    if (useEncryption) dataToLoad = EncryptDecrypt(dataToLoad);
                    loadedData = JsonUtility.FromJson<T>(dataToLoad);

                    Debug.Log($"Load Save On Path : {fullPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError("trying to load data file error" + fullPath + "\n" + e);
                }
            }

            return loadedData;
        }

        public void SaveGame(int saveSlot, GameData data) => Save($"{dataFileName}{saveSlot}", data);

        public void Save(string fileName, object data)
        {
            var fullPath = Path.Combine(dataDirPath, fileName);
            try
            {
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                var dataToStore = JsonUtility.ToJson(data, true);
                if (useEncryption) dataToStore = EncryptDecrypt(dataToStore);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("trying to save data file error" + fullPath + "\n" + e);
            }
        }

        public void DeleteGame(int saveSlot) => Delete($"{dataFileName}{saveSlot}");

        public void Delete(string fileName)
        {
            try
            {
#if UNITY_SWITCH
                NintendoSaveDataHandler.Delete(fileName);
#else
                var fullPath = Path.Combine(dataDirPath, fileName);
                if (!File.Exists(fullPath)) return;
                File.Delete(fullPath);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("trying to delete file error" + fileName + "\n" + e);
            }
        }


        string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }

            return modifiedData;
        }
    }
}
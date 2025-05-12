using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Data.Save;
using System;

public class DataManager
{
    public SaveData CurrentSaveData;
    public Action<int> OnSaveFileChanged;
    private const int MAX_SAVE_SLOTS = 5;
    private JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    private List<SaveData> saveDatas = new List<SaveData>();

    public void Init()
    {
        LoadAllSaveData();
    }

    public string GetSavePath(int saveNum)
    {
        string savePath = Path.Combine(Application.persistentDataPath, $"SaveData{saveNum:D2}.json");
    
        return savePath;
    }

    private bool IsValidIndex(int saveNum)
    {
        return saveNum >= 0 && saveNum < MAX_SAVE_SLOTS;
    }

    public SaveData GetSaveDataWithIndex(int saveNum)
    {
        if(!IsValidIndex(saveNum))
        {
            return null;
        }

        return saveDatas[saveNum];
    }

    public void SetCurrentSaveData(int saveNum)
    {
        if(!IsValidIndex(saveNum))
        {
            return;
        }

        if(saveDatas[saveNum] != null)
        {
            CurrentSaveData = saveDatas[saveNum];
        }

        Debug.Log($"현재 데이터 : {CurrentSaveData.saveNumber}");
    }

    public void SaveDataToJson(int saveNum, SaveData data = null)
    {
        try
        {
            SaveData saveData = data ?? CurrentSaveData;
            if(saveData == null)
            {
                Debug.Log("Data Invalid");
                return;
            }

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, _settings);
            string path = GetSavePath(saveNum);

            File.WriteAllText(path, json);

            saveDatas[saveNum] = saveData;
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void LoadAllSaveData()
    {
        for(int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            saveDatas.Add(LoadData(i));
        }
    }

    public SaveData LoadData(int saveNum)
    {
        try
        {
            string path = GetSavePath(saveNum);
            SaveData data = null;

            if(File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<SaveData>(json, _settings);
            }

            return data;
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    public void DeleteSaveData(int saveNum)
    {
        string path = GetSavePath(saveNum);

        if(!File.Exists(path))
        {
            return;
        }

        try
        {
            File.Delete(path);
            saveDatas[saveNum] = null;
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}

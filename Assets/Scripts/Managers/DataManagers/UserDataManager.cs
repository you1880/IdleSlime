using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.Game;
using Data.Save;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataManager
{
    /*
        인게임 내 이루어지는 모든 데이터 변경은 CurrentSaveData를 조작할 것.
        Save시 CurrentSaveData를 저장.
        Load시 saveDatas[index]에 있는 데이터를 CurrentSaveData로 변경 후 인게임 UI를 이에 맞게 변경
    */
    private const int MAX_SAVE_SLOTS = 5;
    private JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    private List<SaveData> saveDatas = new List<SaveData>();
    public SaveData CurrentSaveData { get; private set; }
    public Action OnUserDataChanged;

    public bool UseMoney(int cost)
    {
        if (CurrentSaveData.money < cost)
        {
            return false;
        }

        CurrentSaveData.money -= cost;
        CurrentSaveData.totalExpenses += cost;

        OnUserDataChanged?.Invoke();

        return true;
    }

    public void AddMoney(int cost)
    {
        CurrentSaveData.money += cost;
        CurrentSaveData.totalEarnings += cost;

        OnUserDataChanged?.Invoke();
    }

    public void IncreaseSkillLevel(int skillId)
    {
        if (skillId >= 0 && skillId < CurrentSaveData.ownedSkills.Count)
        {
            CurrentSaveData.ownedSkills[skillId].skillLevel++;

            OnUserDataChanged?.Invoke();
        }
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
        if (!IsValidIndex(saveNum))
        {
            return null;
        }

        return saveDatas[saveNum];
    }

    public void SetCurrentSaveData(int saveNum)
    {
        if (!IsValidIndex(saveNum))
        {
            return;
        }

        if (saveDatas[saveNum] != null)
        {
            CurrentSaveData = saveDatas[saveNum];
        }
    }

    public void SaveDataToJson(int saveNum, SaveData data = null)
    {
        try
        {
            SaveData saveData = data ?? CurrentSaveData;
            if (saveData == null)
            {
                Debug.Log("Data Invalid");
                return;
            }

            saveData.saveTime = Util.GetCurrentDataTime();
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, _settings);
            string path = GetSavePath(saveNum);

            File.WriteAllText(path, json);

            saveDatas[saveNum] = saveData;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void LoadAllSaveData()
    {
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
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

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<SaveData>(json, _settings);
            }

            return data;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    public void DeleteSaveData(int saveNum)
    {
        string path = GetSavePath(saveNum);

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            File.Delete(path);
            saveDatas[saveNum] = null;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Init()
    {
        LoadAllSaveData();
    }
}

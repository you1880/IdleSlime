using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.Game;
using UnityEngine;

public class GameDataManager
{
    private const int UPGRADE_CLICK_MONEY_ID = 1;
    private Dictionary<int, SlimeData> _slimeDataDict = new Dictionary<int, SlimeData>();
    private Dictionary<int, SkillData> _skillDataDict = new Dictionary<int, SkillData>();
    private GameData _gameData;
    private string _gameDataPath;
    
    public SlimeData GetSlimeDataWithTypeId(int slimeType)
    {
        int defaultType = 0;

        if (_slimeDataDict.TryGetValue(slimeType, out SlimeData data))
        {
            return data;
        }

        return _slimeDataDict[defaultType];
    }

    public SkillData GetSkillDataWithId(int id)
    {
        if (_skillDataDict.TryGetValue(id, out SkillData data))
        {
            return data;
        }

        return new SkillData(-1, 0);
    }

    public int GetSkillUpgradeCost(int id, int level)
    {
        if (!_skillDataDict.TryGetValue(id, out SkillData data))
        {
            return 0;
        }

        if (!(level >= 0 && level < data.costPerLevel.Count))
        {
            return 0;
        }

        return data.costPerLevel[level];
    }

    public float GetUpgradeClickWeight(int level)
    {
        if (!_skillDataDict.TryGetValue(UPGRADE_CLICK_MONEY_ID, out SkillData data))
        {
            return 1.0f;
        }

        if (level >= 0 && level < data.statPerLevel.Count)
        {
            return data.statPerLevel[level];
        }

        return 1.0f;
    }

    private void LoadGameData()
    {
        try
        {
            if (File.Exists(_gameDataPath))
            {
                string json = File.ReadAllText(_gameDataPath);
                _gameData = JsonUtility.FromJson<GameData>(json);

                foreach (SlimeData slimeData in _gameData.slimeDatas)
                {
                    _slimeDataDict.Add(slimeData.slimeType, slimeData);
                }

                foreach (SkillData skillData in _gameData.skillDatas)
                {
                    _skillDataDict.Add(skillData.skillType, skillData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Init()
    {
        _gameDataPath = Path.Combine(Application.streamingAssetsPath, "Data/GameData.json");

        LoadGameData();

        Debug.Log(_skillDataDict[0].costPerLevel.Count);
    }
}

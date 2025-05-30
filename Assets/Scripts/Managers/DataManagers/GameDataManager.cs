using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.Game;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameDataManager
{
    private GameDataSO _gameDataSO;
    private Dictionary<int, SlimeData> _slimeDataDict = new Dictionary<int, SlimeData>();
    private Dictionary<int, SkillData> _skillDataDict = new Dictionary<int, SkillData>();
    private Dictionary<int, AchievementData> _achievementDataDict = new Dictionary<int, AchievementData>();
    
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

        return data.costPerLevel.SafeGetListValue(level);
    }

    public int GetMaxUnlockedSlimeType()
    {
        int currentLevel = Managers.Data.UserDataManager.GetSkillLevel(Define.SkillType.UnlockSlimeType);

        if (!_skillDataDict.TryGetValue((int)Define.SkillType.UnlockSlimeType, out SkillData data))
        {
            return 0; 
        }

        return (int)data.statPerLevel.SafeGetListValue(currentLevel);
    }

    public float GetLevelPerSkillWeight(Define.SkillType skillType, int level)
    {
        if (!_skillDataDict.TryGetValue((int)skillType, out SkillData data))
        {
            return 1.0f;
        }

        return data.statPerLevel.SafeGetListValue(level, 1.0f);
    }

    public AchievementData GetAchievementData(int id)
    {
        if (_achievementDataDict.TryGetValue(id, out AchievementData data))
        {
            //
            if (id == 2)
            {
                data.achievementRequires.ForEach(e => e = e / 60);
            }
            return data;
        }

        return new AchievementData(0, "");
    }

    private void LoadGameData()
    {
        try
        {
            foreach (SlimeData slimeData in _gameDataSO.slimeDatas)
            {
                _slimeDataDict.Add(slimeData.slimeType, slimeData);
            }

            foreach (SkillData skillData in _gameDataSO.skillDatas)
            {
                _skillDataDict.Add(skillData.skillType, skillData);
            }

            foreach (AchievementData achievementData in _gameDataSO.achievementDatas)
            {
                _achievementDataDict.Add(achievementData.achievementId, achievementData);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// GameData.json 준비
    /// StreamingAssets -> Data -> GameData.json 이 되도록 경로를 만들고 함수를 한번 실행
    /// </summary>
    private void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Data/GameData.json");
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameDataSO dataSO = ScriptableObject.CreateInstance<GameDataSO>();
                JsonUtility.FromJsonOverwrite(json, dataSO);

                string savePath = "Assets/GameDataSO.asset";
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(dataSO, savePath);
                AssetDatabase.SaveAssets();
#endif
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Init()
    {
        _gameDataSO = Managers.Resource.Load<GameDataSO>("GameDataSO");

        LoadGameData();
    }
}

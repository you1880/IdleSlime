using System.Collections;
using System.Collections.Generic;
using Data.Game;
using UnityEngine;

namespace Data.Save
{
    [System.Serializable]
    public class SaveData
    {
        public int saveNumber;
        public string saveTime;
        public int money;
        public int totalEarnings;
        public int totalExpenses;
        public float playTime;
        public List<OwnedSlime> ownedSlimes;
        public List<OwnedSkill> ownedSkills;

        public SaveData(int num, string time, List<OwnedSlime> slimes, List<OwnedSkill> skills)
        {
            saveNumber = num;
            saveTime = time;
            money = 0;
            totalEarnings = 0;
            totalExpenses = 0;
            playTime = 0.0f;
            ownedSlimes = slimes;
            ownedSkills = skills;
        }
    }

    [System.Serializable]
    public class OwnedSlime
    {
        public int slimeType;
        public int slimeCount;
        public int slimeEnhancementLevel;

        public OwnedSlime(int sType, int count)
        {
            slimeType = sType;
            slimeCount = count;
            slimeEnhancementLevel = 0;
        }
    }

    [System.Serializable]
    public class OwnedSkill
    {
        public int skillType;
        public int skillLevel;

        public OwnedSkill(int type, int level)
        {
            skillType = type;
            skillLevel = level;
        }
    }
}

namespace Data.Game
{
    [System.Serializable]
    public class GameData
    {
        public List<SlimeData> slimeDatas;
        public List<SkillData> skillDatas;
        public List<AchievementData> achievementDatas;

        public GameData()
        {
            slimeDatas = new List<SlimeData>();
            skillDatas = new List<SkillData>();
            achievementDatas = new List<AchievementData>();
        }
    }

    [System.Serializable]
    public class SkillData
    {
        public int skillType;
        public int maxSkillLevel;
        public string skillName;
        public string skillLore;
        public List<int> costPerLevel;
        public List<float> statPerLevel;

        public SkillData(int type, int level)
        {
            skillType = type;
            maxSkillLevel = level;
            costPerLevel = new List<int>();
            statPerLevel = new List<float>();
        }
    }

    [System.Serializable]
    public class SlimeData
    {
        public int slimeType;
        public int idleMoney;
        public int clickMoney;
        public int slimePrice;
        public List<SlimeEnhanceData> slimeEnhanceDatas;

        public SlimeData(int type, int idle, int click, int price, List<SlimeEnhanceData> enhanceDatas)
        {
            slimeType = type;
            idleMoney = idle;
            clickMoney = click;
            slimePrice = price;
            slimeEnhanceDatas = enhanceDatas;
        }
    }

    [System.Serializable]
    public class SlimeEnhanceData
    {
        public float successRate;
        public float failRate;
        public int requireSlimeCount;
        public int requireMoney;

        public SlimeEnhanceData(float sRate, float fRate, int count, int money)
        {
            successRate = sRate;
            failRate = fRate;
            requireSlimeCount = count;
            requireMoney = money;
        }
    }

    [System.Serializable]
    public class AchievementData
    {
        public int achievementId;
        public string achievementName;
        public List<int> achievementRequires;

        public AchievementData(int id, string name)
        {
            achievementId = id;
            achievementName = name;
            achievementRequires = new List<int>();
        }
    }
}
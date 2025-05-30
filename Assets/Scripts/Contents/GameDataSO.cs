using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "ScriptableObjects/GameDataSO", order = 1)]
public class GameDataSO : ScriptableObject
{
    public List<Data.Game.SlimeData> slimeDatas;
    public List<Data.Game.SkillData> skillDatas;
    public List<Data.Game.AchievementData> achievementDatas;

    public void LoadFromGameData(Data.Game.GameData gameData)
    {
        slimeDatas = gameData.slimeDatas;
        skillDatas = gameData.skillDatas;
        achievementDatas = gameData.achievementDatas;
    }
}

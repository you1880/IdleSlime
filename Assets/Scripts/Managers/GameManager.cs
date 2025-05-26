using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Save;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    private const float BASE_X = 1280.0f;
    private const float BASE_Y = 720.0f;
    private const float MIN_X = -7.0f;
    private const float MAX_X = 7.0f;
    private const float MIN_Y = -2.5f;
    private const float MAX_Y = 1.5f;
    private const float IDLE_DELAY_TIME = 5.0f;
    private const string INSUFFICIENT_MONEY_MESSAGE = "돈이 부족합니다.";
    private List<float> _clickMultipliersByEnhanceLevel = new List<float> { 1.0f, 1.2f, 1.5f, 2.0f, 3.5f };
    private GameDataManager gameDataManager => Managers.Data.GameDataManager;
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private SaveData userData => userDataManager.CurrentSaveData;
    private Coroutine _idleIncomeCoroutine;
    public Action OnResolutionChanged;

    public List<float> GetMoveRanges()
    {
        float width = Screen.width / BASE_X;
        float height = Screen.height / BASE_Y;

        return new List<float> { MIN_X * width, MAX_X * width, MIN_Y * height, MAX_Y * height };
    }

    public OwnedSlime GetOwnedSlimeInUserData(int slimeType)
    {
        foreach (OwnedSlime ownedSlime in userData.ownedSlimes)
        {
            if (slimeType == ownedSlime.slimeType)
            {
                return ownedSlime;
            }
        }

        return null;
    }

    /// <summary>
    /// 슬라임 타입에 맞는 클릭 당 얻는 골드 * 클릭 당 얻는 골드 추가 스킬의 레벨당 상승량 * 슬라임 강화 등급에 맞는 가중치
    /// </summary>
    /// <param name="slimeType">슬라임 타입</param>
    public int GetTotalAmountClickMoney(int slimeType)
    {
        SlimeData slimeData = gameDataManager.GetSlimeDataWithTypeId(slimeType);
        OwnedSlime ownedSlime = GetOwnedSlimeInUserData(slimeType);
        int totalAmount = slimeData.clickMoney;

        if (ownedSlime == null)
        {
            return totalAmount;
        }

        int skillLevel = userDataManager.GetSkillLevel(Define.SkillType.UpgradeClickMoney);
        float skillWeight = gameDataManager.GetUpgradeClickWeight(skillLevel);
        float enhanceWeight = (ownedSlime.slimeEnhancementLevel < 0 || ownedSlime.slimeEnhancementLevel >= _clickMultipliersByEnhanceLevel.Count) ?
            1.0f : _clickMultipliersByEnhanceLevel[ownedSlime.slimeEnhancementLevel];

        totalAmount = Mathf.RoundToInt(totalAmount * skillWeight * enhanceWeight);

        return totalAmount;
    }

    public void AddClickMoneyToData(int slimeType)
    {
        int totalAmount = GetTotalAmountClickMoney(slimeType);

        userDataManager.AddMoney(totalAmount);
    }

    public SlimeEnhanceData GetEnhanceData(int slimeType, int currentEnhanceLevel)
    {
        SlimeData slimeData = gameDataManager.GetSlimeDataWithTypeId(slimeType);

        if (currentEnhanceLevel < 0 || currentEnhanceLevel >= slimeData.slimeEnhanceDatas.Count)
        {
            return slimeData.slimeEnhanceDatas[0];
        }

        return slimeData.slimeEnhanceDatas[currentEnhanceLevel];
    }

    public float GetFinalEnhanceChance(float successRate)
    {
        float addChance = gameDataManager.GetLevelPerSkillWeight(Define.SkillType.AddEnhancementChance, userDataManager.GetSkillLevel(Define.SkillType.AddEnhancementChance));

        return successRate + addChance >= 1.0f ? 1.0f : successRate + addChance;
    }

    public void EnhanceSlimeLevel(int slimeType)
    {
        OwnedSlime ownedSlime = GetOwnedSlimeInUserData(slimeType);

        if (ownedSlime == null)
        {
            string msg = "보유하지 않은 슬라임입니다.";
            ShowResult(msg, Define.EffectSoundType.Fail);

            return;
        }

        SlimeEnhanceData slimeEnhanceData = GetEnhanceData(slimeType, ownedSlime.slimeEnhancementLevel);
        if (ownedSlime.slimeEnhancementLevel >= (int)Define.GradeType.GradeSP)
        {
            //최고 레벨
            return;
        }

        if (ownedSlime.slimeCount <= slimeEnhanceData.requireSlimeCount)
        {
            //요구 슬라임보다 최소 1개이상 많아야 함.
            string msg = "강화에 필요한 슬라임이 부족합니다.";
            ShowResult(msg, Define.EffectSoundType.Fail);

            return;
        }

        if (!userDataManager.UseMoney(slimeEnhanceData.requireMoney))
        {
            ShowResult(INSUFFICIENT_MONEY_MESSAGE, Define.EffectSoundType.Fail);

            return;
        }

        float finalEnhanceChance = GetFinalEnhanceChance(slimeEnhanceData.successRate);
        bool isSuccess = UnityEngine.Random.Range(0.0f, 1.0f) <= finalEnhanceChance;
        ownedSlime.slimeCount -= slimeEnhanceData.requireSlimeCount;

        if (isSuccess)
        {
            string msg = "강화에 성공하셨습니다.";
            ShowResult(msg, Define.EffectSoundType.Grow);
        }
        else
        {
            string msg = "강화에 실패하여 등급이 하락했습니다.";
            ShowResult(msg, Define.EffectSoundType.Fail);
        }

        userDataManager.AddOrSubSlimeEnhanceLevel(ownedSlime, isSuccess);
    }

    public void BuySlime(int slimeType, int cost)
    {
        if (slimeType <= (int)Define.SlimeType.MinSlimeType || slimeType >= (int)Define.SlimeType.MaxSlimeType)
        {
            return;
        }

        if (userDataManager.UseMoney(cost))
        {
            Managers.Sound.PlayEffectSound(Define.EffectSoundType.Buy);
            userDataManager.AddSlimeInOwnedSlimes(slimeType);
        }
        else
        {
            Managers.Sound.PlayEffectSound(Define.EffectSoundType.Fail);
        }
    }

    public void BuySkillUpgrade(int skillId)
    {
        if (skillId < 0 || skillId >= (int)Define.SkillType.MaxSkillType)
        {
            return;
        }

        int level = userData.ownedSkills[skillId].skillLevel;
        int cost = gameDataManager.GetSkillUpgradeCost(skillId, level);

        if (userDataManager.UseMoney(cost))
        {
            string msg = "스킬을 구매하였습니다.";
            userDataManager.IncreaseSkillLevel(skillId);

            ShowResult(msg, Define.EffectSoundType.Buy);
        }
        else
        {
            ShowResult(INSUFFICIENT_MONEY_MESSAGE, Define.EffectSoundType.Buy);
        }
    }

    public void CollectIdleIncome()
    {
        int income = GetTotalIdleIncome();

        _idleIncomeCoroutine = Managers.RunCoroutine(IdleIncomeRoutine(income));
    }

    public void StopIdleIncomeRoutine()
    {
        if (Managers.Instance != null)
        {
            Managers.TerminateCoroutine(_idleIncomeCoroutine);
        }
    }

    public int GetTotalIdleIncome()
    {
        int totalIncome = 0;

        foreach (OwnedSlime ownedSlime in userData.ownedSlimes)
        {
            int idleMoney = gameDataManager.GetSlimeDataWithTypeId(ownedSlime.slimeType).idleMoney;
            int slimeCount = ownedSlime.slimeCount;

            totalIncome += (idleMoney * slimeCount);
        }

        return totalIncome;
    }

    private IEnumerator IdleIncomeRoutine(int income)
    {
        while (true)
        {
            yield return new WaitForSeconds(IDLE_DELAY_TIME);

            userDataManager.AddMoney(income);
        }
    }

    private void ShowResult(string msg, Define.EffectSoundType effectSoundType)
    {
        Managers.UI.ShowMessageBoxUI(msg, null, true);
        Managers.Sound.PlayEffectSound(effectSoundType);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementSlot
{
    private const string TROPHY_IMAGE_NAME = "Trophy";
    private const string ACHIEVEMENT_TEXT_NAME = "AchievementText";
    private const string STATUS_TEXT = "AchievementStatus";
    public Image trophyImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;

    public AchievementSlot(Image img, TextMeshProUGUI name, TextMeshProUGUI statTxt)
    {
        trophyImage = img;
        nameText = name;
        statusText = statTxt;
    }

    public static AchievementSlot CreateSlot(GameObject slotObject)
    {
        Image trophyImage = Util.FindChild<Image>(slotObject, TROPHY_IMAGE_NAME, true);
        TextMeshProUGUI nameText = Util.FindChild<TextMeshProUGUI>(slotObject, ACHIEVEMENT_TEXT_NAME, true);
        TextMeshProUGUI statusText = Util.FindChild<TextMeshProUGUI>(slotObject, STATUS_TEXT, true);

        return new AchievementSlot(trophyImage, nameText, statusText);
    }
}

public class UI_Achievement : UI_Base
{
    private enum GameObjects
    {
        AchievementSlot0,
        AchievementSlot1,
        AchievementSlot2,
        AchievementSlot3
    }

    private enum Buttons
    {
        ExitButton
    }

    private List<AchievementSlot> _achievementSlots = new List<AchievementSlot>();

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private (int, int) GetTrophyGrade(int id)
    {
        Data.Save.SaveData saveData = Managers.Data.UserDataManager.CurrentSaveData;
        List<int> requireValues = Managers.Data.GameDataManager.GetAchievementData(id).achievementRequires;
        int grade = 0;
        int checkValue = 0;

        switch (id)
        {
            case 0:
                checkValue = saveData.totalEarnings;
                break;
            case 1:
                checkValue = saveData.totalExpenses;
                break;
            case 2:
                checkValue = (int)saveData.playTime;
                break;
            case 3:
                checkValue = Managers.Data.GameDataManager.GetMaxUnlockedSlimeType();
                break;
            default:
                break;
        }

        foreach (int val in requireValues)
        {
            if (checkValue < val)
            {
                break;
            }

            grade++;
        }

        return (checkValue, grade);
    }

    private void InitSlotImageAndText(AchievementSlot slot, int id)
    {
        if (slot == null)
        {
            return;
        }

        AchievementData data = Managers.Data.GameDataManager.GetAchievementData(id);
        (int, int) gradeValue = GetTrophyGrade(id);
        int curValue = gradeValue.Item1;
        int grade = gradeValue.Item2;

        slot.nameText.text = $"{data.achievementName}";
        if (grade == (int)Define.GradeType.GradeSP)
        {
            slot.statusText.text = "-";
        }
        else
        {
            slot.statusText.text = $"{curValue} / {data.achievementRequires.SafeGetListValue(grade, 0)}";
        }

        if (grade < (int)Define.GradeType.GradeC)
        {
            slot.trophyImage.sprite = null;
            slot.trophyImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            slot.trophyImage.sprite = Managers.Resource.LoadTrophySprite(grade);
            slot.trophyImage.color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
        }
    }   

    private void GetSlotElements()
    {
        foreach (GameObjects slot in Enum.GetValues(typeof(GameObjects)))
        {
            GameObject slotObject = GetObject((int)slot);
            AchievementSlot achievementSlot = AchievementSlot.CreateSlot(slotObject);
            InitSlotImageAndText(achievementSlot, (int)slot);
        }
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        BindButtonEvent();
        GetSlotElements();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot
{
    private const string ICON_PANEL_NAME = "IconPanel";
    private const string LOCK_IMAGE_NAME = "ButtonLock";
    private const string COST_TEXT_NAME = "CostText";
    private const string UPGRADE_BUY_BUTTON_NAME = "BuyButton";
    public GameObject iconPanel;
    public GameObject lockObject;
    public TextMeshProUGUI costText;
    public Button upgradeBuyButton;

    public UpgradeSlot(GameObject panel, GameObject lockObj, TextMeshProUGUI txt, Button btn)
    {
        iconPanel = panel;
        lockObject = lockObj;
        costText = txt;
        upgradeBuyButton = btn;
    }

    public static UpgradeSlot CreateUpgradeSlot(GameObject slotObject)
    {
        GameObject panel = Util.FindChild(slotObject, ICON_PANEL_NAME, true);
        GameObject lockObject = Util.FindChild(slotObject, LOCK_IMAGE_NAME, true);
        TextMeshProUGUI costText = Util.FindChild<TextMeshProUGUI>(slotObject, COST_TEXT_NAME, true);
        Button buyButton = Util.FindChild<Button>(slotObject, UPGRADE_BUY_BUTTON_NAME, true);

        return new UpgradeSlot(panel, lockObject, costText, buyButton);
    }
}

public class UI_Upgrade : UI_Base
{
    private enum GameObjects
    {
        SlimeSlotUpgrade,
        ClickMoneyUpgrade,
        IdleMoneyUpgrade,
        EnhancementChanceUpgrade
    }

    private enum Images
    {
        UpgradeLorePanel
    }

    private enum Texts
    {
        SkillNameText,
        SkillLoreText
    }

    private enum Buttons
    {
        ExitButton
    }

    private GameDataManager gameDataManager => Managers.Data.GameDataManager;
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private SaveData userData => Managers.Data.UserDataManager.CurrentSaveData;
    private Vector2 _offset = new Vector2(175.0f, -175.0f);
    private List<UpgradeSlot> _upgradeSlots = new List<UpgradeSlot>();
    private GameObject _skillLorePanel;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _loreText;

    private void OnBuyButtonClicked(PointerEventData data)
    {
        for (int i = 0; i < _upgradeSlots.Count; i++)
        {
            if (_upgradeSlots[i].upgradeBuyButton.gameObject == data.pointerClick.gameObject)
            {
                Managers.Game.BuySkillUpgrade(i);
            }
        }
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.Resource.Destroy(this.gameObject);
    }

    private void InitUpgradeSlots()
    {
        foreach (GameObjects slot in Enum.GetValues(typeof(GameObjects)))
        {
            int id = (int)slot;
            GameObject slotObject = GetObject(id);
            UpgradeSlot upgradeSlot = UpgradeSlot.CreateUpgradeSlot(slotObject);

            _upgradeSlots.Add(upgradeSlot);
        }
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < _upgradeSlots.Count; i++)
        {
            EnableOrDisableSlot(_upgradeSlots[i], i);
        }
    }

    private void EnableOrDisableSlot(UpgradeSlot slot, int id)
    {
        SkillData skillData = gameDataManager.GetSkillDataWithId(id);
        int currentUserSkillLevel = userData.ownedSkills[id].skillLevel;

        if (currentUserSkillLevel + 1 > skillData.maxSkillLevel) // Disable Slot
        {
            slot.lockObject.SetActive(true);
            slot.costText.text = "-";
        }
        else // Enable Slot
        {
            int cost = gameDataManager.GetSkillUpgradeCost(id, currentUserSkillLevel);

            slot.lockObject.SetActive(false);
            slot.costText.text = $"{cost:N0}";
            slot.upgradeBuyButton.gameObject.BindEvent(OnBuyButtonClicked);
        }
    }

    private void InitPanelInfo(int id)
    {
        if (_skillLorePanel == null)
        {
            return;
        }

        SkillData data = gameDataManager.GetSkillDataWithId(id);

        _nameText.text = data.skillName;
        _loreText.text = data.skillLore;
    }

    private void OnPanelEnter(PointerEventData data)
    {
        if (_skillLorePanel == null)
        {
            return;
        }

        _skillLorePanel.SetActive(true);

        for (int i = 0; i < _upgradeSlots.Count; i++)
        {
            if (data.pointerEnter.gameObject == _upgradeSlots[i].iconPanel)
            {
                InitPanelInfo(i);

                break;
            }
        }
    }

    private void OnPanelExit(PointerEventData data)
    {
        if (_skillLorePanel == null || !_skillLorePanel.activeSelf)
        {
            return;
        }

        _skillLorePanel.SetActive(false);
    }

    private void MoveUIElement()
    {
        if (_skillLorePanel == null)
        {
            return;
        }

        _skillLorePanel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x + _offset.x,
            Input.mousePosition.y + _offset.y,
            0.0f
        ));
    }

    private void BindPanelEvent()
    {
        _skillLorePanel = GetImage((int)Images.UpgradeLorePanel).gameObject;
        _nameText = GetText((int)Texts.SkillNameText);
        _loreText = GetText((int)Texts.SkillLoreText);

        foreach (UpgradeSlot slot in _upgradeSlots)
        {
            slot.iconPanel.BindEvent(OnPanelEnter, Define.UIEvent.PointerEnter);
            slot.iconPanel.BindEvent(OnPanelExit, Define.UIEvent.PointerExit);
        }

        _skillLorePanel.SetActive(false);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    public override void Init()
    {
        BindUIElements();
        InitUpgradeSlots();
        UpdateSlots();
        BindButtonEvent();
        BindPanelEvent();
    }

    private void Update()
    {
        MoveUIElement();
    }

    private void OnEnable()
    {
        userDataManager.OnUserDataChanged -= UpdateSlots;
        userDataManager.OnUserDataChanged += UpdateSlots;
    }

    private void OnDisable()
    {
        if (userDataManager != null)
        {
            userDataManager.OnUserDataChanged -= UpdateSlots;
        }
    }
}

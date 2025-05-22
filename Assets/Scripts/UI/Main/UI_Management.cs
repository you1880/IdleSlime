using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Management : UI_Base
{
    private enum GameObjects
    {
        SlimeManagement,
        Lock,
        UserManagement
    }

    private enum Images
    {
        SlimeImage,
        GradeImage
    }

    private enum Texts
    {
        SlimeCount,
        ClickMoneyText,
        IdleMoneyText,
        EnhanceSuccessText,
        EnhanceFailText,
        EnhanceCountText,
        EnhancMoneyText,
        UnlockSlimeLevel,
        UnlockSlimeLore,
        UpgradeClickMoneyLevel,
        UpgradeClickMoneyLore,
        UpgradeIdleMoneyLevel,
        UpgradeIdleMoneyLore,
        AddEnhancementChanceLevel,
        AddEnhancementChanceLore,
        TotalEarningsText,
        TotalExpensesText,
        TotalPlayTimeText,
        TotalIdleMoneyText,
        TotalSlimeCountText
    }

    private enum Buttons
    {
        LeftManageButton,
        RightManageButton,
        LeftButton,
        RightButton,
        EnhanceButton,
        ExitButton
    }

    #region SlimePanel
    private GameObject _slimeManagePanel;
    private GameObject _lockObject;
    private Image _slimeImage;
    private Image _slimeGradeImage;
    private TextMeshProUGUI _slimeCountText;
    private TextMeshProUGUI _clickMoneyText;
    private TextMeshProUGUI _idleMoneyText;
    private TextMeshProUGUI _successChanceText;
    private TextMeshProUGUI _failChanceText;
    private TextMeshProUGUI _requireCountText;
    private TextMeshProUGUI _requireMoneyText;
    private Button _slimeLeftButton;
    private Button _slimeRightButton;
    private int _currentSlimeType = 1;
    private int _maxUnlockedSlimeType;
    #endregion

    #region UserPanel
    private GameObject _userManagePanel;
    private TextMeshProUGUI _unlockLevel;
    private TextMeshProUGUI _unlockLore;
    private TextMeshProUGUI _clickMoneyLevel;
    private TextMeshProUGUI _clickMoneyLore;
    private TextMeshProUGUI _idleMoneyLevel;
    private TextMeshProUGUI _idleMoneyLore;
    private TextMeshProUGUI _addChanceLevel;
    private TextMeshProUGUI _addChanceLore;
    private TextMeshProUGUI _totalEarningsText;
    private TextMeshProUGUI _totalExpensesText;
    private TextMeshProUGUI _totalPlayTimeText;
    private TextMeshProUGUI _totalIdleMoneyText;
    private TextMeshProUGUI _totalSlimeCountText;
    #endregion

    private const int NON_GRADE = -1;
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private GameDataManager gameDataManager => Managers.Data.GameDataManager;
    private SaveData userData => Managers.Data.UserDataManager.CurrentSaveData;
    private Button _manageRightButton;
    private Button _manageLeftButton;
    private bool _isUserPanel = false;

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void OnManageButtonClicked(PointerEventData data)
    {
        _isUserPanel = !_isUserPanel;

        _userManagePanel.SetActive(_isUserPanel);
        _slimeManagePanel.SetActive(!_isUserPanel);
    }

    #region SlimePanelFunction
    private void ShowLockObject()
    {
        if (_currentSlimeType > _maxUnlockedSlimeType)
        {
            _lockObject.SetActive(true);
        }
        else
        {
            _lockObject.SetActive(false);
        }
    }

    private void SetSlimeSpriteAndGrade(int slimeEnhancementLevel)
    {
        if (slimeEnhancementLevel == NON_GRADE)
        {
            _slimeGradeImage.sprite = null;
        }
        else
        {
            _slimeGradeImage.sprite = Managers.Resource.LoadSlimeGradeSprite(slimeEnhancementLevel);

        }

        _slimeImage.sprite = Managers.Resource.LoadSlimeSprite(_currentSlimeType);
    }

    private void SetSlimeDataText(int count)
    {
        int idleMoney = Managers.Data.GameDataManager.GetSlimeDataWithTypeId(_currentSlimeType).idleMoney;

        _clickMoneyText.text = $"{Managers.Game.GetTotalAmountClickMoney(_currentSlimeType):N0}(Click)";
        _idleMoneyText.text = $"{idleMoney:N0}(Idle)";
        _slimeCountText.text = $"보유 수 : {count}";
    }

    private void SetEnhancePanel(int slimeEnhancementLevel)
    {
        SlimeEnhanceData slimeEnhanceData = Managers.Game.GetEnhanceData(_currentSlimeType, slimeEnhancementLevel);

        _successChanceText.text = $"성공 확률 : {Managers.Game.GetFinalEnhanceChance(slimeEnhanceData.successRate)}";
        _failChanceText.text = $"실패(하락) 확률 : {slimeEnhanceData.failRate}";
        _requireCountText.text = $"필요한 슬라임 수 : {slimeEnhanceData.requireSlimeCount}";
        _requireMoneyText.text = $"필요한 골드 : {slimeEnhanceData.requireMoney:N0}";
    }

    private void InitSlimePanel()
    {
        ShowLockObject();

        int slimeEnhancementLevel = NON_GRADE;
        int count = 0;

        if (_currentSlimeType <= _maxUnlockedSlimeType)
        {
            foreach (OwnedSlime ownedSlime in userData.ownedSlimes)
            {
                if (_currentSlimeType == ownedSlime.slimeType)
                {
                    slimeEnhancementLevel = ownedSlime.slimeEnhancementLevel;
                    count = ownedSlime.slimeCount;

                    break;
                }
            }
        }

        SetSlimeSpriteAndGrade(slimeEnhancementLevel);
        SetSlimeDataText(count);
        SetEnhancePanel(slimeEnhancementLevel);
    }

    private void OnSlimeLeftButtonClicked(PointerEventData data)
    {
        _currentSlimeType = (_currentSlimeType - 1 <= (int)Define.SlimeType.MinSlimeType) ?
            (int)Define.SlimeType.SlimeTwelve : _currentSlimeType - 1;

        InitSlimePanel();
    }

    private void OnSlimeRightButtonClicked(PointerEventData data)
    {
        _currentSlimeType = (_currentSlimeType + 1 >= (int)Define.SlimeType.MaxSlimeType) ?
            (int)Define.SlimeType.SlimeOne : _currentSlimeType + 1;

        InitSlimePanel();
    }

    private void OnEnhanceButtonClicked(PointerEventData data)
    {
        Managers.Game.EnhanceSlimeLevel(_currentSlimeType);
    }
    #endregion

    private void InitSkillPanel()
    {
        int unlockLevel = userDataManager.GetSkillLevel(Define.SkillType.UnlockSlimeType);
        int upgradeClickLevel = userDataManager.GetSkillLevel(Define.SkillType.UpgradeClickMoney);
        int upgradeIdleLevel = userDataManager.GetSkillLevel(Define.SkillType.UpgradeIdleMoney);
        int addChanceLevel = userDataManager.GetSkillLevel(Define.SkillType.AddEnhancementChance);

        _unlockLevel.text = $"Lv.{unlockLevel}";
        _unlockLore.text = $"현재 해금 단계 : {(int)gameDataManager.GetLevelPerSkillWeight(Define.SkillType.UnlockSlimeType, unlockLevel)}";

        _clickMoneyLevel.text = $"Lv.{upgradeClickLevel}";
        _clickMoneyLore.text = $"클릭 당 {gameDataManager.GetLevelPerSkillWeight(Define.SkillType.UpgradeClickMoney, upgradeClickLevel)}배 증가";

        _idleMoneyLevel.text = $"Lv.{upgradeIdleLevel}";
        _idleMoneyLore.text = $"{gameDataManager.GetLevelPerSkillWeight(Define.SkillType.UpgradeIdleMoney, upgradeIdleLevel)}배 추가 획득";

        _addChanceLevel.text = $"Lv.{addChanceLevel}";
        _addChanceLore.text = $"강화 성공 확률 {gameDataManager.GetLevelPerSkillWeight(Define.SkillType.AddEnhancementChance, addChanceLevel)}% 추가";
    }

    private void InitHistoryPanel()
    {
        int count = 0;
        foreach (OwnedSlime ownedSlime in userData.ownedSlimes)
        {
            count += ownedSlime.slimeCount;
        }

        (int, int) playTime = userDataManager.GetTotalPlayTime();

        _totalEarningsText.text = $"누적 획득 골드 : {userData.totalEarnings}";
        _totalExpensesText.text = $"누적 사용 골드 : {userData.totalExpenses}";
        _totalIdleMoneyText.text = $"5초 당 얻는 총 골드 : {Managers.Game.GetTotalIdleIncome()}";
        _totalPlayTimeText.text = $"총 플레이 시간 : {playTime.Item1}시간 {playTime.Item2}분";
        _totalSlimeCountText.text = $"총 슬라임 갯수 : {count}";
    }

    private void InitUserPanel()
    {
        InitSkillPanel();
        InitHistoryPanel();
    }

    private void UpdatePanelInfo()
    {
        if (_isUserPanel)
        {
            InitUserPanel();
        }
        else
        {
            InitSlimePanel();
        }
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetSlimePanelUIElements()
    {
        _lockObject = GetObject((int)GameObjects.Lock);
        _slimeImage = GetImage((int)Images.SlimeImage);
        _slimeGradeImage = GetImage((int)Images.GradeImage);
        _slimeCountText = GetText((int)Texts.SlimeCount);
        _clickMoneyText = GetText((int)Texts.ClickMoneyText);
        _idleMoneyText = GetText((int)Texts.IdleMoneyText);
        _successChanceText = GetText((int)Texts.EnhanceSuccessText);
        _failChanceText = GetText((int)Texts.EnhanceFailText);
        _requireCountText = GetText((int)Texts.EnhanceCountText);
        _requireMoneyText = GetText((int)Texts.EnhancMoneyText);
        _slimeLeftButton = GetButton((int)Buttons.LeftButton);
        _slimeRightButton = GetButton((int)Buttons.RightButton);
        _maxUnlockedSlimeType = Managers.Data.GameDataManager.GetMaxUnlockedSlimeType();
    }

    private void GetUserPanelUIElements()
    {
        _unlockLevel = GetText((int)Texts.UnlockSlimeLevel);
        _unlockLore = GetText((int)Texts.UnlockSlimeLore);
        _clickMoneyLevel = GetText((int)Texts.UpgradeClickMoneyLevel);
        _clickMoneyLore = GetText((int)Texts.UpgradeClickMoneyLore);
        _idleMoneyLevel = GetText((int)Texts.UpgradeIdleMoneyLevel);
        _idleMoneyLore = GetText((int)Texts.UpgradeIdleMoneyLore);
        _addChanceLevel = GetText((int)Texts.AddEnhancementChanceLevel);
        _addChanceLore = GetText((int)Texts.AddEnhancementChanceLore);
        _totalEarningsText = GetText((int)Texts.TotalEarningsText);
        _totalExpensesText = GetText((int)Texts.TotalExpensesText);
        _totalPlayTimeText = GetText((int)Texts.TotalPlayTimeText);
        _totalIdleMoneyText = GetText((int)Texts.TotalIdleMoneyText);
        _totalSlimeCountText = GetText((int)Texts.TotalSlimeCountText);
    }

    private void GetUIElements()
    {
        _slimeManagePanel = GetObject((int)GameObjects.SlimeManagement);
        _userManagePanel = GetObject((int)GameObjects.UserManagement);
        _manageLeftButton = GetButton((int)Buttons.LeftManageButton);
        _manageRightButton = GetButton((int)Buttons.RightManageButton);

        GetSlimePanelUIElements();
        GetUserPanelUIElements();
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.EnhanceButton).gameObject.BindEvent(OnEnhanceButtonClicked);
        _manageLeftButton.gameObject.BindEvent(OnManageButtonClicked);
        _manageRightButton.gameObject.BindEvent(OnManageButtonClicked);
        _slimeLeftButton.gameObject.BindEvent(OnSlimeLeftButtonClicked);
        _slimeRightButton.gameObject.BindEvent(OnSlimeRightButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitSlimePanel();
        InitUserPanel();

        _userManagePanel.SetActive(false);
    }

    private void OnEnable()
    {
        Managers.Data.UserDataManager.OnUserDataChanged -= UpdatePanelInfo;
        Managers.Data.UserDataManager.OnUserDataChanged += UpdatePanelInfo;
    }

    private void OnDisable()
    {
        Managers.Data.UserDataManager.OnUserDataChanged -= UpdatePanelInfo;
    }
}

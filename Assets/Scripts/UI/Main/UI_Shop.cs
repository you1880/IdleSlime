using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Base
{
    private enum GameObjects
    {
        ButtonLock,
        LockText
    }

    private enum Texts
    {
        CostText
    }   

    private enum Images
    {
        SlimeImage
    }

    private enum Buttons
    {
        ExitButton,
        LeftItemButton,
        RightItemButton,
        BuyButton
    }

    private const int MAX_SLIME_TYPE = 12;
    private const int MIN_SLIME_TYPE = 1;
    private GameObject _lockObject;
    private GameObject _lockText;
    private Image _currentSlimeImage;
    private TextMeshProUGUI _costText;
    private int _currentSimeType = 1;
    private int _currentSlimeCost;
    private int _maxUnlockedSlimeType;

    private void ShowButtonLockObjects()
    {
        if (_currentSimeType > _maxUnlockedSlimeType)
        {
            _lockObject.SetActive(true);
            _lockText.SetActive(true);
        }
        else
        {
            _lockObject.SetActive(false);
            _lockText.SetActive(false);
        }
    }

    private void ShowSlimeCost()
    {
        _currentSlimeCost = Managers.Data.GameDataManager.GetSlimeDataWithTypeId(_currentSimeType).slimePrice;

        _costText.text = $"{_currentSlimeCost:N0}";
    }

    private void ChangeSlimeSprite()
    {
        _currentSlimeImage.sprite = Managers.Resource.LoadSlimeSprite(_currentSimeType);
    }

    private void InitShopInfo()
    {
        ChangeSlimeSprite();
        ShowButtonLockObjects();
        ShowSlimeCost();
    }

    private void OnBuyButtonClicked(PointerEventData data)
    {
        Managers.Game.BuySlime(_currentSimeType, _currentSlimeCost);
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void OnRightButtonClicked(PointerEventData data)
    {
        _currentSimeType = (_currentSimeType + 1) > MAX_SLIME_TYPE ? 1 : _currentSimeType + 1;
        InitShopInfo();
    }

    private void OnLeftButtonClicked(PointerEventData data)
    {
        _currentSimeType = (_currentSimeType - 1) < MIN_SLIME_TYPE ? MAX_SLIME_TYPE : _currentSimeType - 1;
        InitShopInfo();
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _lockObject = GetObject((int)GameObjects.ButtonLock);
        _lockText = GetObject((int)GameObjects.LockText);
        _currentSlimeImage = GetImage((int)Images.SlimeImage);
        _costText = GetText((int)Texts.CostText);

        _lockObject.SetActive(false);
        _lockText.SetActive(false);

        _maxUnlockedSlimeType = Managers.Data.GameDataManager.GetMaxUnlockedSlimeType();
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.LeftItemButton).gameObject.BindEvent(OnLeftButtonClicked);
        GetButton((int)Buttons.RightItemButton).gameObject.BindEvent(OnRightButtonClicked);
        GetButton((int)Buttons.BuyButton, false).gameObject.BindEvent(OnBuyButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitShopInfo();
    }
}

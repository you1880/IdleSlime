using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Menu : UI_Base
{
    private enum GameObjects
    {
        ButtonNamePanel
    }

    private enum Texts
    {
        ButtonName
    }

    private enum Buttons
    {
        ShopButton,
        UpgradeButton,
        SlimeManagementButton,
        AchievementButton,
        SettingButton
    }
    
    private Dictionary<string, string> _buttonNameDict = new Dictionary<string, string>
    {
        {"ShopButton", "상점"},
        {"UpgradeButton", "강화"},
        {"SlimeManagementButton", "관리"},
        {"AchievementButton", "업적"},
        {"SettingButton", "설정"}
    };
    private UIManager uIManager => Managers.UI;
    private Vector2 _offset = new Vector2(-50.0f, -50.0f);
    private GameObject _buttonNamePanel;
    private TextMeshProUGUI _buttonNameText;

    private void OnShopButtonClicked(PointerEventData data)
    {
        uIManager.ShowMenuUI<UI_Shop>();
    }

    private void OnUpgradeButtonClicked(PointerEventData data)
    {
        uIManager.ShowMenuUI<UI_Upgrade>();
    }

    private void OnManagementButtonClicked(PointerEventData data)
    {
        uIManager.ShowMenuUI<UI_Management>();
    }

    private void OnAchievementButtonClicked(PointerEventData data)
    {
        uIManager.ShowMenuUI<UI_Achievement>();
    }

    private void OnSettingButtonClicked(PointerEventData data)
    {
        uIManager.ShowMenuUI<UI_Setting>();
    }

    private void OnButtonEnter(PointerEventData data)
    {
        _buttonNamePanel.SetActive(true);

        if(_buttonNameDict.TryGetValue(data.pointerEnter.name, out string text))
        {
            _buttonNameText.text = text;
        }
    }

    private void OnButtonExit(PointerEventData data)
    {
        _buttonNamePanel.SetActive(false);
        _buttonNameText.text = "";
    }

    private void MoveUIElements()
    {
        if(_buttonNamePanel == null)
        {
            return;
        }

        _buttonNamePanel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x + _offset.x,
            Input.mousePosition.y + _offset.y,
            0.0f
        ));
        
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvents()
    {
        List<Action<PointerEventData>> buttonActions = 
            new List<Action<PointerEventData>> { OnShopButtonClicked, OnUpgradeButtonClicked, OnManagementButtonClicked, OnAchievementButtonClicked, OnSettingButtonClicked };
        for(int i = 0; i < 5; i++)
        {
            GameObject buttonObject = GetButton(i).gameObject;

            buttonObject.BindEvent(buttonActions[i]);
            buttonObject.BindEvent(OnButtonEnter, Define.UIEvent.PointerEnter);
            buttonObject.BindEvent(OnButtonExit, Define.UIEvent.PointerExit);
        }
    }

    private void InitNamePanel()
    {
        _buttonNamePanel = GetObject((int)GameObjects.ButtonNamePanel);
        _buttonNameText = GetText((int)Texts.ButtonName);
        _buttonNamePanel.SetActive(false);
    }

    public override void Init()
    {
        BindUIElements();
        BindButtonEvents();
        InitNamePanel();
    }

    private void Update()
    {
        MoveUIElements();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Base
{
    public enum GameObjects
    {
        LobbySelect
    }

    private enum Images
    {
        LobbySelectImageLeft,
        LobbySelectImageRight
    }

    public enum Buttons
    {
        LobbyStartButton,
        LobbyLoadButton,
        LobbyQuitButton
    }

    private RectTransform _selectButtonImageRect;
    private Image _selectImageLeft;
    private Image _selectImageRight;
    private int[] _buttonLocations = { 50, -75, -200 };
    private string[] _buttonColorHex = { "#C4FFD1", "#C2F0FF", "#FFE3E3" };
    private int _currentButton = (int)Buttons.LobbyStartButton;

    private void OnStartButtonClicked(PointerEventData data)
    {
        UI_SaveLoad ui = Managers.UI.ShowUI<UI_SaveLoad>("UI_SaveLoad");
        ui.SetCurrentMode(0);
    }

    private void OnLoadButtonClicked(PointerEventData data)
    {
        UI_SaveLoad ui = Managers.UI.ShowUI<UI_SaveLoad>("UI_SaveLoad");
        ui.SetCurrentMode(1);
    }

    private void OnQuitButtonClicked(PointerEventData data)
    {
        //TODO
        //Application 종료
        Debug.Log("Quit Button");
    }

    private void OnButtonEnter(PointerEventData data)
    {
        if(!Enum.TryParse(data.pointerEnter.name, out Buttons enteredButton))
        {
            return;
        }

        int selectButton = (int)enteredButton;

        if(_currentButton == selectButton)
        {
            return;
        }

        _currentButton = selectButton;
        _selectButtonImageRect.anchoredPosition = new Vector3(
            _selectButtonImageRect.anchoredPosition.x, 
            _buttonLocations[selectButton],
            0.0f
        );

        _selectImageLeft.color = _selectImageRight.color = Util.GetColorFromHex(_buttonColorHex[selectButton]);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.LobbyStartButton).gameObject.BindEvent(OnStartButtonClicked);
        GetButton((int)Buttons.LobbyLoadButton).gameObject.BindEvent(OnLoadButtonClicked);
        GetButton((int)Buttons.LobbyQuitButton).gameObject.BindEvent(OnQuitButtonClicked);

        for(int i = 0; i < 3; i++)
        {
            GetButton(i).gameObject.BindEvent(OnButtonEnter, Define.UIEvent.PointerEnter);
        }
    }

    private void GetUIElements()
    {
        _selectButtonImageRect = GetObject((int)GameObjects.LobbySelect).GetComponent<RectTransform>();
        _selectImageLeft = GetImage((int)Images.LobbySelectImageLeft);
        _selectImageRight = GetImage((int)Images.LobbySelectImageRight);
    }

    public override void Init()
    {
        BindUIElements();
        BindButtonEvent();
        GetUIElements();
    }
}

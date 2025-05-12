using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MessageBox : UI_Base
{
    private enum Texts
    {
        MessageBoxText,
    }

    private enum Buttons
    {
        OkButton,
        NoButton
    }

    private TextMeshProUGUI _msgBoxText;
    private string _text;
    private Action _callback;

    public void SetMessageBox(string text, Action onCompleted)
    {
        _text = text;
        _callback = onCompleted;
    }

    private void OnOkButtonClicked(PointerEventData data)
    {
        _callback?.Invoke();
        Managers.Resource.Destroy(this.gameObject);
    }

    private void OnNoButtonClicked(PointerEventData data)
    {
        Managers.Resource.Destroy(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.OkButton).gameObject.BindEvent(OnOkButtonClicked);
        GetButton((int)Buttons.NoButton).gameObject.BindEvent(OnNoButtonClicked);
    }

    private void GetUIElements()
    {
        _msgBoxText = GetText((int)Texts.MessageBoxText);
        _msgBoxText.text = _text;
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }
}

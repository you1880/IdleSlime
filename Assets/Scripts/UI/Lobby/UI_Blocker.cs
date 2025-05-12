using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Blocker : UI_Base
{
    private enum Texts
    {
        LoadingText
    }

    private TextMeshProUGUI _loadingText;


    public void SetText(string text)
    {
        if(_loadingText == null)
        {
            return;
        }

        _loadingText.text = text;
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        _loadingText = GetText((int)Texts.LoadingText);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
    }
}

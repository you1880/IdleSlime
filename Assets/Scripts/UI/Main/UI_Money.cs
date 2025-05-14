using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Money : UI_Base
{
    private enum Texts
    {
        MoneyText
    }

    private TextMeshProUGUI _moneyText;

    private void SetMoneyText()
    {
        _moneyText.text = $"{Managers.Data.CurrentSaveData.money:N0}";
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        _moneyText = GetText((int)Texts.MoneyText);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        SetMoneyText();
    }
}

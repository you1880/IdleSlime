using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private UI_Base _openedMenuUI;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");

            if(root == null)
            {
                root = new GameObject { name = "@UI_Root" };
                Canvas canvas = root.GetOrAddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
            }

            return root;
        }
    }

    public T ShowUI<T>(string name = null) where T : UI_Base
    {
        if(string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject obj = Managers.Resource.Instantiate($"UI/{name}");

        if(obj == null)
        {
            return null;
        }
        
        T ui = obj.GetOrAddComponent<T>();
        obj.transform.SetParent(Root.transform, false);

        return ui;
    }

    public T ShowMenuUI<T>(string name = null) where T : UI_Base
    {
        if (_openedMenuUI != null)
        {
            CloseUI(_openedMenuUI.gameObject);
        }
        
        _openedMenuUI = ShowUI<T>(name);

        return _openedMenuUI as T;
    }

    public UI_MessageBox ShowMessageBoxUI(string msg, Action onCompleted = null, bool isConfirmMode = false)
    {
        UI_MessageBox messageBox = ShowUI<UI_MessageBox>();
        messageBox.SetMessageBox(msg, onCompleted, isConfirmMode);

        return messageBox;
    }

    public void CloseUI(GameObject ui)
    {
        if (ui == null)
        {
            return;
        }

        if (ui == _openedMenuUI)
        {
            _openedMenuUI = null;
        }

        Managers.Resource.Destroy(ui);
        ui = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
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

    public void CloseUI(GameObject ui)
    {
        if(ui == null)
        {
            return;
        }

        Managers.Resource.Destroy(ui);
        ui = null;
    }
}

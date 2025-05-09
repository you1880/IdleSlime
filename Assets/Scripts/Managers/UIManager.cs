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
                Canvas canvas = Util.GetOrAddComponent<Canvas>(root);
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
            }

            return root;
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    UIManager _ui = new UIManager();

    public static UIManager UI { get { return Instance._ui; } }

    private static void Init()
    {
        if(_instance == null)
        {
            GameObject manager = GameObject.Find("@GameManager");

            if(manager == null)
            {
                manager = new GameObject { name = "@GameManager" };
                manager.AddComponent<Managers>();
            }

            DontDestroyOnLoad(manager);

            _instance = manager.GetComponent<Managers>();
        }
    }

    void Start()
    {
        Init();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEX _scene = new SceneManagerEX();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEX Scene { get { return Instance._scene; } } 
    public static SoundManager Sound { get { return Instance._sound; } }
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

            _instance._data.Init();
            _instance._sound.Init();
        }
    }

    public static Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public static void TerminateCoroutine(Coroutine coroutine)
    {
        if(coroutine == null)
        {
            return;
        }

        Instance.StopCoroutine(coroutine);
    }

    void Start()
    {
        Init();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    private DataManager _data = new DataManager();
    private GameManager _game = new GameManager();
    private InputManager _input = new InputManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEX _scene = new SceneManagerEX();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static GameManager Game { get { return Instance._game; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEX Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    private static bool _isApplicationQuitting = false;
    
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    private static void Init()
    {
        if (_isApplicationQuitting)
        {
            return;
        }
        
        if (_instance == null)
        {
            GameObject manager = GameObject.Find("@Managers");

            if (manager == null)
            {
                manager = new GameObject { name = "@Managers" };
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
        if (coroutine == null)
        {
            return;
        }

        Instance.StopCoroutine(coroutine);
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        _input.OnUpdate();

        if (_scene.CurrentScene is MainScene)
        {
            _data.UserDataManager.CalCurrentSavePlayTime();
        }
    }
}

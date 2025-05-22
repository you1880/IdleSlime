using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerEX
{
    private const float WAIT_TIME = 1.0f;
    private const float FADE_DELAY_TIME = 1.0f;
    private const float ALPHA_OPAQUE = 1.0f;
    private const float ALPHA_TRANSPARENT = 0.0f;
    private GameObject _blocker;
    private Image _blockImage;
    private UI_Blocker _blockerUI;

    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadNextScene(Define.SceneType sceneType = Define.SceneType.Main)
    {
        string sceneName = GetSceneName(sceneType);

        Managers.RunCoroutine(LoadSceneAsync(sceneName));
    }

    private void CreateBlockerUI()
    {
        if(Managers.UI.Root.transform.Find("UI_Blocker") == null)
        {
            _blockerUI = Managers.UI.ShowUI<UI_Blocker>();
        }

        _blocker = _blockerUI.gameObject;
        _blockImage = _blocker.GetComponent<Image>();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float startTime = Time.time;

        yield return Managers.RunCoroutine(Fade(ALPHA_TRANSPARENT, ALPHA_OPAQUE));
        SetBlockText("로딩중...");

        while(operation.progress < 0.9f)
        {
            yield return null;
        }

        float elapsed = Time.time - startTime;

        if(elapsed < WAIT_TIME)
        {
            yield return new WaitForSeconds(WAIT_TIME - elapsed);
        }

        SetBlockText("로딩 완료!");
        yield return new WaitForSeconds(WAIT_TIME / 2.0f);

        //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        operation.allowSceneActivation = true;
        yield return null;

        yield return Managers.RunCoroutine(FadeIn());
    }

    public IEnumerator Fade(float start, float end)
    {
        CreateBlockerUI();
        
        float elapsed = 0.0f;

        Color c = _blockImage.color;
        _blockImage.color = new Color(c.r, c.g, c.b, start);

        while(elapsed < FADE_DELAY_TIME)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FADE_DELAY_TIME);
            float alpha = Mathf.Lerp(start, end, t);

            _blockImage.color = new Color(c.r, c.g, c.b, alpha);

            yield return null;
        }
        
        yield return null;
    }

    public IEnumerator FadeIn()
    {
        yield return Managers.RunCoroutine(Fade(ALPHA_OPAQUE, ALPHA_TRANSPARENT));

        Clear();
    }

    private void SetBlockText(string text)
    {
        if(_blockerUI == null)
        {
            return;
        }

        _blockerUI.SetText(text);
    }

    private void Clear()
    {
        Managers.Resource.Destroy(_blocker);

        _blocker = null;
        _blockerUI = null;
        _blockImage = null;
    }

    private void FadeOut()
    {
        Managers.RunCoroutine(Fade(ALPHA_TRANSPARENT, ALPHA_OPAQUE));
    }

    private string GetSceneName(Define.SceneType type)
    {
        string name = Enum.GetName(typeof(Define.SceneType), type);

        return name;
    }
}

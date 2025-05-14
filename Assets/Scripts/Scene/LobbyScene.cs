using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override void Clear() {}

    public override void Init()
    {
        
        StartCoroutine(LobbySceneInit());
    }

    private IEnumerator LobbySceneInit()
    {
        yield return StartCoroutine(Managers.Scene.FadeIn());
        
        Managers.Sound.PlayBGM();
        Managers.UI.ShowUI<UI_Lobby>();
    }
}

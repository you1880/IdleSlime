using System.Collections;
using System.Collections.Generic;

public class Define
{
    public enum MouseEvent
    {
        LClick,
        RClick,
    }

    public enum UIEvent
    {
        Click,
        PointerEnter,
        PointerExit,
    }

    public enum SceneType
    {
        Unknown,
        Lobby,
        Main
    }

    public enum SoundType
    {
        Bgm,
        Effect,
        Count, // Count용
    }
}

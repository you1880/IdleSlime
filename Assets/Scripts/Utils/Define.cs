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

    public enum EffectSoundType
    {
        Button,
        Buy,
        Grow,
        Fail,
        Touch,
        Unlock
    }

    public enum SlimeState
    {
        Idle,
        Move,
        Touch
    }

    public enum SlimeType
    {
        MinSlimeType = 0,
        SlimeOne,
        SlimeTwo,
        SlimeThree,
        SlimeFour,
        SlimeFive,
        SlimeSix,
        SlimeSeven,
        SlimeEight,
        SlimeNine,
        SlimeTen,
        SlimeEleven,
        SlimeTwelve,
        MaxSlimeType = 13
    }

    public enum SkillType
    {
        UnlockSlimeType = 0,
        UpgradeClickMoney = 1,
        UpgradeIdleMoney = 2,
        AddEnhancementChance = 3,
        MaxSkillType = 4
    }

    public enum GradeType
    {
        GradeC = 1,
        GradeB = 2,
        GradeA = 3,
        GradeS = 4,
        GradeSP = 5
    }
}

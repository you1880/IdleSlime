using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static void BindEvent(this GameObject obj, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(obj, action, eventType);
    }

    public static T GetOrAddComponent<T>(this GameObject obj) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(obj);
    }

    public static T SafeGetListValue<T>(this List<T> list, int index, T defaultVal = default)
    {
        return (index >= 0 && index < list.Count) ? list[index] : defaultVal;
    }
}

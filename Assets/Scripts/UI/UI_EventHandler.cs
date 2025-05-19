using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnPointerEnterHandler = null;
    public Action<PointerEventData> OnPointerExitHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnPointerEnterHandler != null)
        {
            OnPointerEnterHandler.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(OnPointerExitHandler != null)
        {
            OnPointerExitHandler.Invoke(eventData);
        }
    }
}

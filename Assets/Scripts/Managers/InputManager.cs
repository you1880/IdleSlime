using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    private const float CLICK_DELAY = 0.5f;
    private float _lastClick = 0.0f;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(pos.x, pos.y));

            if (colliders == null)
            {
                return;
            }

            Slime slime = null;
            int slimeType = 0;
            
            if (colliders.Length == 1)
            {
                slime = colliders[0].GetComponent<Slime>();
            }
            else
            {
                foreach (Collider2D col in colliders)
                {
                    
                    Slime stmp = col.GetComponent<Slime>();
                    Debug.Log($"COL : {stmp.SlimeType}");
                    if (slimeType < stmp.SlimeType)
                    {
                        slime = stmp;
                        slimeType = stmp.SlimeType;
                    }
                }
            }

            if (slime != null)
            {
                if (Time.time - _lastClick < CLICK_DELAY)
                {
                    return;
                }

                _lastClick = Time.time;
                slime.OnMouseEvent(Define.MouseEvent.LClick);
            }
        }
    }
}

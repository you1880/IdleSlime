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
            Collider2D collider = Physics2D.OverlapPoint(new Vector2(pos.x, pos.y));

            if (collider == null)
            {
                return;
            }

            Slime slime = collider.GetComponent<Slime>();

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

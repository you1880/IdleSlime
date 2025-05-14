using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private int _slimeType;

    public void SetSlimeType(int slimeType)
    {
        _slimeType = slimeType;
    }

    private void InitSlime()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _spriteRenderer.sprite = Managers.Resource.LoadSlimeSprite(_slimeType);
    }

    void Start()
    {
        InitSlime();
    }
}

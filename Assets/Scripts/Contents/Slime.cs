using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private const float SLIME_MOVE_SPEED = 1.0f;
    private const float MIN_IDLE_DLEAY_TIME = 2.0f;
    private const float MAX_IDLE_DELAY_TIME = 7.0f;
    private const float CHECK_DESTINATION_DISTANCE = 0.1f;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Define.SlimeState _slimeState = Define.SlimeState.Idle;
    private IReadOnlyList<float> _canMoveRanges;
    private Vector2 _destPos;
    private int _slimeType;
    private float _moveDelayTime;

    public Define.SlimeState SlimeState
    {
        get { return _slimeState; }
        set
        {
            _slimeState = value;
            SetSlimeAnimation();

            switch (_slimeState)
            {
                case Define.SlimeState.Idle:
                    SetIdleState();
                    break;
                case Define.SlimeState.Move:
                    FlipSprite();
                    break;
                case Define.SlimeState.Touch:
                    SetTouchState();
                    break;
            }
        }
    }

    private void SetSlimeAnimation()
    {
        if (_animator == null)
        {
            return;
        }

        string animatioName = Enum.GetName(typeof(Define.SlimeState), _slimeState);
        _animator.Play(animatioName, 0, 0);
    }

    private void FlipSprite()
    {
        if (transform.position.x > _destPos.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void SetIdleState()
    {
        _moveDelayTime = UnityEngine.Random.Range(MIN_IDLE_DLEAY_TIME, MAX_IDLE_DELAY_TIME);

        _destPos.x = UnityEngine.Random.Range(_canMoveRanges[0], _canMoveRanges[1]);
        _destPos.y = UnityEngine.Random.Range(_canMoveRanges[2], _canMoveRanges[3]);
    }

    private void UpdateIdle()
    {
        _moveDelayTime -= Time.deltaTime;

        if (_moveDelayTime <= 0.0f)
        {
            SlimeState = Define.SlimeState.Move;
        }
    }

    private void UpdateMove()
    {
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, _destPos, SLIME_MOVE_SPEED * Time.deltaTime);

        transform.position = newPos;

        if (Vector2.Distance(newPos, _destPos) < CHECK_DESTINATION_DISTANCE)
        {
            SlimeState = Define.SlimeState.Idle;
        }
    }

    private void SetTouchState()
    {
        Managers.Game.AddClickMoneyToData(_slimeType);

        StartCoroutine(SwitchIdleAfterTouch());
    }

    private IEnumerator SwitchIdleAfterTouch()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        SlimeState = Define.SlimeState.Idle;
    }

    public void OnMouseEvent(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.LClick)
        {
            SlimeState = Define.SlimeState.Touch;
            Managers.Sound.PlayEffectSound(Define.EffectSoundType.Touch);
        }
    }

    public void SetSlimeType(int slimeType)
    {
        _slimeType = slimeType;
    }

    private void InitSlime()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _spriteRenderer.sprite = Managers.Resource.LoadSlimeSprite(_slimeType);
        _canMoveRanges = Managers.Game.GetMoveRanges();

        SetIdleState();
    }

    private void Start()
    {
        InitSlime();
    }

    private void Update()
    {
        switch (_slimeState)
        {
            case Define.SlimeState.Idle:
                UpdateIdle();
                break;
            case Define.SlimeState.Move:
                UpdateMove();
                break;
            case Define.SlimeState.Touch:
                break;
        }
    }
}

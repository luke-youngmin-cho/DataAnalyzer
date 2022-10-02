using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineForPlayer : StateMachineBase
{
    public enum StateTypes
    {
        Idle,
        Move,
        Jump,
        Fall,
        Land,
        Attack,
        Crouch,
        Hurt,
        Die
    }

    private bool _isInterating;
    public bool IsInteracting
    {
        get
        {
            return _isInterating;
        }
        set
        {
            if (value)
            {
                _movement.IsMovable = false;
            }
            else
            {
                _movement.IsMovable = true;
            }
            _isInterating = value;
        }
    }

    public bool IsInteractable
    {
        get
        {
            return  IsInteracting == false &&
                    (StateType == StateTypes.Idle ||
                     StateType == StateTypes.Move);
        }
    }

    private Movement _movement;
    private void Awake()
    {
        StartCoroutine(E_Init());
    }

    IEnumerator E_Init()
    {
        StateType = StateTypes.Idle;
        _movement = GetComponent<Movement>();
        RefreshStates();
        ShortCutSettings.SetUpDefaultStateMachineSettings(this);
        IsReady = true;
        yield return null;
    }

    protected override void RefreshStates()
    {
        var values = Enum.GetValues(typeof(StateTypes));
        foreach (StateTypes value in values)
            AddState(value);
        States[StateTypes.Move].Active();
        Current = States[StateTypes.Move];
        StateType = StateTypes.Move;
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        FixedUpdateState();
    }

}
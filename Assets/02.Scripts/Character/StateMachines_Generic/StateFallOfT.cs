using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFall<T> : StateBase<T> where T : Enum
{
    private Movement _movement;
    private GroundDetector _groundDetector;
    private AnimationManagerBase _animationManager;
    private Rigidbody _rb;
    private float _fallStartPosY;
    private float _heightToLand = 3.0f;

    public StateFall(T stateType, T[] nextTargets, T canExecuteConditionMask, StateMachineBase<T> machine) 
        : base(stateType, nextTargets, canExecuteConditionMask, machine)
    {
        _movement = machine.GetComponent<Movement>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _rb = machine.GetComponent<Rigidbody>();
    }


    public override void Active()
    {
        base.Active();
        _movement.IsMovable = false;
    }

    public override T Update()
    {
        T nextStateType = StateType;
        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                {
                    _animationManager.SetBool("DoFall", true);
                    _fallStartPosY = _rb.position.y;
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnFall"))
                    {
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Casting:
                {
                    _animationManager.SetBool("DoFall", false);
                    MoveNext();
                }
                break;
            case IState.Commands.OnAction:
                {
                    if (_groundDetector.IsDetected == true)
                    {
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Finish:
                {
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilFinished:
                {
                    MoveNext();
                }
                break;
            case IState.Commands.Finished:
                {
                    for (int i = 0; i < FinishConditions.Length; i++)
                    {
                        if (FinishConditions[i]())
                        {
                            nextStateType = NextTargets[i];
                            break;
                        }
                    }
                }
                break;
            case IState.Commands.Error:
                break;
            case IState.Commands.WaitUntilErrorCleared:
                break;
            default:
                break;
        }
        return nextStateType;
    }

    protected override void DefineFinishConditions()
    {
        FinishConditions = new Func<bool>[2];
        FinishConditions[0] = () => _fallStartPosY - _rb.position.y > _heightToLand;
        FinishConditions[1] = () => true;
    }
}

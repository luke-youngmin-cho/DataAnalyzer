using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove<T> : StateBase<T> where T : Enum
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private CharacterBase _character;

    public StateMove(T stateType, T[] nextTargets, T CanExecuteConditionMask, StateMachineBase<T> machine) 
        : base(stateType, nextTargets, CanExecuteConditionMask, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _character = machine.GetComponent<CharacterBase>();
    }

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = true;
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
                    _animationManager.SetBool("DoMove", true);
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnMove"))
                    {
                        _animationManager.SetBool("DoMove", false);
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Casting:
                {
                    MoveNext();
                }
                break;
            case IState.Commands.OnAction:
                {
                    _animationManager.SetFloat("MoveBlend", _movement.Speed / _character.MoveSpeedMax);
                }
                break;
            case IState.Commands.Finish:
                MoveNext();
                break;
            case IState.Commands.WaitUntilFinished:
                MoveNext();
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

}

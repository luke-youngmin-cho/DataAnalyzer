using System;
using UnityEngine;

public class StateHurt<T> : StateBase<T> where T : Enum
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;

    public StateHurt(T stateType, T[] nextTargets, T canExecuteConditionMask, StateMachineBase<T> machine) 
        : base(stateType, nextTargets, canExecuteConditionMask, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        DefineFinishConditions();
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
                    _animationManager.Play("Hurt");
                    MoveNext();
                }
                break;
            case IState.Commands.Casting:
                MoveNext();
                break;
            case IState.Commands.OnAction:
                {
                    if (_animationManager.GetCurrentNormalizedTime() > 0.9f)
                        MoveNext();
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

using System;
using UnityEngine;

public class StateDie<T> : StateBase<T> where T : Enum
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;

    public StateDie(T stateType, T[] nextTargets, T canExecuteConditionMask, StateMachineBase<T> machine)
        : base(stateType, nextTargets, canExecuteConditionMask, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
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
                    _animationManager.Play("Die");
                    MoveNext();
                }
                break;
            case IState.Commands.Casting:
                MoveNext();
                break;
            case IState.Commands.OnAction:
                {
                    // nothing to do
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

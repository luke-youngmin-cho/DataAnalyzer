using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdleForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    public StateIdleForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine)
        : base(stateType, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
    }

    public override bool Available => true;

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = true;
    }

    public override dynamic Update()
    {
        dynamic nextStateType = StateType;

        switch (Command)
        {
            case Commands.Idle:
                break;
            case Commands.Prepare:
                {
                    _animationManager.Play("Move");
                    MoveNext();
                }
                break;
            case Commands.Casting:
                {
                    float currentMoveBlend = _animationManager.GetFloat("MoveBlend");
                    if (currentMoveBlend <= 0.00f)
                    {
                        MoveNext();
                    }
                    else
                    {
                        _animationManager.SetFloat("MoveBlend", currentMoveBlend -= Time.deltaTime);
                    }
                }
                break;
            case Commands.OnAction:
                MoveNext();
                break;
            case Commands.Finish:
                MoveNext();
                break;
            case Commands.WaitUntilFinished:
                MoveNext();
                break;
            case Commands.Finished:
                break;
            case Commands.Error:
                break;
            case Commands.WaitUntilErrorCleared:
                break;
            default:
                break;
        }

        return nextStateType;
    }
}

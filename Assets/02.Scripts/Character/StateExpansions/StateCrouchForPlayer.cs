using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCrouchForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    public StateCrouchForPlayer(StateMachineForPlayer.StateTypes stateType,
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
                    _animationManager.Play("Crouch");
                    MoveNext();
                }
                break;
            case Commands.Casting:
                MoveNext();
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

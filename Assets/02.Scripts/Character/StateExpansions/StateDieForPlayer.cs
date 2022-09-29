using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDieForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private float _animationTime;
    private float _animationTimer;
    public StateDieForPlayer(StateMachineForPlayer.StateTypes stateType,
                             StateMachineBase machine)
        : base(stateType, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _animationTime = _animationManager.GetClipTime("Die");
    }

    public override bool Available => true;

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = false;
    }

    public override dynamic Update()
    {
        StateMachineForPlayer.StateTypes nextStateType = StateType;

        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                {
                    _animationManager.Play("Die");
                    _animationTimer = _animationTime;
                    MoveNext();
                }
                break;
            case IState.Commands.Casting:
                MoveNext();
                break;
            case IState.Commands.OnAction:
                {
                    if (_animationTimer < 0)
                        MoveNext();
                    else
                        _animationTimer -= Time.deltaTime;
                }
                break;
            case IState.Commands.Finish:
                MoveNext();
                break;
            case IState.Commands.WaitUntilFinished:
                MoveNext();
                break;
            case IState.Commands.Finished:
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

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
            case Commands.Idle:
                break;
            case Commands.Prepare:
                {
                    _animationManager.Play("Die");
                    _animationTimer = _animationTime;
                    MoveNext();
                }
                break;
            case Commands.Casting:
                MoveNext();
                break;
            case Commands.OnAction:
                {
                    if (_animationTimer < 0)
                        MoveNext();
                    else
                        _animationTimer -= Time.deltaTime;
                }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHurtForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private float _animationTime;
    private float _animationTimer;
    public StateHurtForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine)
        : base(stateType, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _animationTime = _animationManager.GetClipTime("Hurt"); 
    }

    public override bool Available => Machine.StateType == StateMachineForPlayer.StateTypes.Idle ||
                                      Machine.StateType == StateMachineForPlayer.StateTypes.Move ||
                                      Machine.StateType == StateMachineForPlayer.StateTypes.Fall ||
                                      Machine.StateType == StateMachineForPlayer.StateTypes.Jump ||
                                      Machine.StateType == StateMachineForPlayer.StateTypes.Crouch;

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
                    _animationManager.Play("Hurt");
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
                nextStateType = StateMachineForPlayer.StateTypes.Move;
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

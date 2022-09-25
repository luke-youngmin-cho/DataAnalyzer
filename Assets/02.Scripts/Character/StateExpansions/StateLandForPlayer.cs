using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLandForPlayer : StateBase
{    
    private Movement _movement;
    private GroundDetector _groundDetector;
    private AnimationManagerBase _animationManager;
    public StateLandForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine)
        : base(stateType, machine)
    {
        _movement = machine.GetComponent<Movement>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
        _animationManager = machine.GetComponent<AnimationManagerBase>();
    }

    public override bool Available => _animationManager.IsPreviousAnimationFinished &&
                                      Machine.StateType == StateMachineForPlayer.StateTypes.Fall;

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = false;
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
                    _animationManager.SetBool("DoLand", true);
                    MoveNext();
                }
                break;
            case Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnLand"))
                    {
                        MoveNext();
                    }
                }
                break;
            case Commands.Casting:
                {
                    _animationManager.SetBool("DoLand", false);
                    MoveNext();
                }
                break;
            case Commands.OnAction:
                {
                    if (_animationManager.GetCurrentNormalizedTime() > 0.7f)
                    {
                        MoveNext();
                    }
                }
                break;
            case Commands.Finish:
                {
                    MoveNext();
                }
                break;
            case Commands.WaitUntilFinished:
                {
                    MoveNext();
                }
                break;
            case Commands.Finished:
                {
                    nextStateType = StateMachineForPlayer.StateTypes.Move;
                }
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

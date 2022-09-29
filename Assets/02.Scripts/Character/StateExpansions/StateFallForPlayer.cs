using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFallForPlayer : StateBase
{    
    private Movement _movement;
    private GroundDetector _groundDetector;
    private AnimationManagerBase _animationManager;
    private Rigidbody _rb;
    private float _fallStartPosY;
    private float _heightToLand = 3.0f;

    public StateFallForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine)
        : base(stateType, machine)
    {
        _movement = machine.GetComponent<Movement>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _rb = machine.GetComponent<Rigidbody>();
    }

    public override bool Available => _animationManager.IsPreviousAnimationFinished &&
                                      (Machine.StateType == StateMachineForPlayer.StateTypes.Idle ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Move ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Crouch ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Jump);

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
                    if (_rb.position.y < _heightToLand)
                        nextStateType = StateMachineForPlayer.StateTypes.Move;
                    else
                        nextStateType = StateMachineForPlayer.StateTypes.Land;
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

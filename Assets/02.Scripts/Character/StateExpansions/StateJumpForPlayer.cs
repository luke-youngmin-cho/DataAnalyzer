using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateJumpForPlayer : StateBase
{
    private Movement _movement;
    private GroundDetector _groundDetector;
    private AnimationManagerBase _animationManager;
    private Rigidbody _rigidbody;
    
    public StateJumpForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine) 
        : base(stateType, machine)
    {
        _movement = machine.GetComponent<Movement>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _rigidbody = machine.GetComponent<Rigidbody>();
    }

    public override bool Available
    {
        get =>  _animationManager.IsPreviousAnimationFinished &&
                _groundDetector.IsDetected &&
               (Machine.StateType == StateMachineForPlayer.StateTypes.Idle ||
                Machine.StateType == StateMachineForPlayer.StateTypes.Move ||
                Machine.StateType == StateMachineForPlayer.StateTypes.Crouch);
    }

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = false;
    }

    public override void Deactive()
    {
        base.Deactive();
        _animationManager.SetBool("DoJump", false);
        _animationManager.SetBool("DoLand", false);
        _animationManager.SetBool("JumpError", false);
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
                    _animationManager.SetBool("DoJump", true);
                    MoveNext();
                }
                break;
            case Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnJump"))
                    {
                        _animationManager.SetBool("DoJump", false);
                        _movement.ResetVelocityY();
                        _rigidbody.AddForce(force: Vector3.up * PlayerInfo.JumpForce,
                                            mode: ForceMode.VelocityChange);
                        MoveNext();
                    }
                }
                break;
            case Commands.Casting:
                {
                    if (_groundDetector.IsDetected == false)
                    {
                        MoveNext();
                    }
                }
                break;
            case Commands.OnAction:
                {
                    if (_rigidbody.velocity.y < 0 ||
                        _groundDetector.IsDetected)
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
                    if (_groundDetector.IsDetected)
                    {
                        nextStateType = StateMachineForPlayer.StateTypes.Move;
                    }
                    else
                    {
                        nextStateType = StateMachineForPlayer.StateTypes.Fall;
                    }
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

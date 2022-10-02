using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateJump<T> : StateBase<T> where T : Enum
{
    private Movement _movement;
    private GroundDetector _groundDetector;
    private AnimationManagerBase _animationManager;
    private CharacterBase _character;
    private Rigidbody _rigidbody;

    public StateJump(T stateType, T[] nextTargets, T CanExecuteConditionMask, StateMachineBase<T> machine) 
        : base(stateType, nextTargets, CanExecuteConditionMask, machine)
    {
        _movement = machine.GetComponent<Movement>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _character = machine.GetComponentInChildren<CharacterBase>();
        _rigidbody = machine.GetComponent<Rigidbody>();
        DefineFinishConditions();
    }

    public override bool Available => base.Available && 
                                      _groundDetector.IsDetected;

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = false;
    }

    public override void Deactive()
    {
        base.Deactive();
        _animationManager.SetBool("DoJump", false);
        _animationManager.SetBool("JumpError", false);
    }

    public override T Update()
    {
        dynamic nextStateType = StateType;
        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                {
                    _animationManager.SetBool("DoJump", true);
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnJump"))
                    {
                        _animationManager.SetBool("DoJump", false);
                        _movement.ResetVelocityY();
                        _rigidbody.AddForce(force: Vector3.up * _character.JumpForce,
                                            mode: ForceMode.VelocityChange);
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Casting:
                {
                    if (_groundDetector.IsDetected == false)
                    {
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.OnAction:
                {
                    if (_rigidbody.velocity.y < 0 ||
                        _groundDetector.IsDetected)
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

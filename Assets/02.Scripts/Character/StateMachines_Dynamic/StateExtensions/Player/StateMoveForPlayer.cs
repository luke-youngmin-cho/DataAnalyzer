using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMoveForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private GroundDetector _groundDetector;
    private CharacterBase _character;
    public StateMoveForPlayer(StateMachineForPlayer.StateTypes stateType,
                              StateMachineBase machine) 
        : base(stateType, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _character = machine.GetComponent<CharacterBase>();
        _groundDetector = machine.GetComponentInChildren<GroundDetector>();
    }

    public override bool Available => _animationManager.IsPreviousAnimationFinished;

    public override void Active()
    {
        base.Active();
        _movement.IsMovable = true;
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
                    _animationManager.SetBool("DoMove", true);
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilPrepared:
                {
                    if (_animationManager.GetBool("OnMove"))
                    {
                        _animationManager.SetBool("DoMove", false);
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Casting:
                {                    
                    MoveNext();
                }
                break;
            case IState.Commands.OnAction:
                {
                    _animationManager.SetFloat("MoveBlend", _movement.Speed / _character.MoveSpeedMax);

                    if (_groundDetector.IsDetected == false)
                        nextStateType = StateMachineForPlayer.StateTypes.Fall;
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

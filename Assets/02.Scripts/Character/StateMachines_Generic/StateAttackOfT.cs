using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack<T> : StateBase<T> where T : Enum
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private CharacterBase _character;
    private Rigidbody _rb;
    private bool _onCombo;
    private int _comboCount;

    public StateAttack(T stateType, T[] nextTargets, T canExecuteConditionMask, StateMachineBase<T> machine)
        : base(stateType, nextTargets, canExecuteConditionMask, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _character = machine.GetComponent<CharacterBase>();
        _rb = machine.GetComponent<Rigidbody>();
    }

    public override void Active()
    {
        base.Active();
        _movement.Stop();
        _movement.IsMovable = false;
    }

    public override void Deactive()
    {
        base.Deactive();
        _animationManager.SetBool("DoAttack", false);
        _animationManager.SetBool("DoAttackCombo", false);
        _animationManager.DisableCombo();
    }

    public override T Update()
    {
        T nextStateType = StateType;

        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                {
                    _comboCount = 0;
                    _animationManager.SetInt("ComboCount", _comboCount);
                    _animationManager.SetBool("WeaponEquiped", _character.WeaponEquiped);
                    _animationManager.SetBool("DoAttack", true);
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilPrepared:
                {
                    if (_animationManager.IsPreviousAnimationFinished)
                    {
                        MoveNext();
                    }
                }
                break;
            case IState.Commands.Casting:
                {
                    if (_animationManager.IsComboAvailable)
                    {
                        // first attack
                        if (_onCombo == false)
                        {
                            _animationManager.SetBool("DoAttack", false);
                            MoveNext();
                        }
                        // combo attack
                        else
                        {
                            _onCombo = false;
                            _animationManager.SetBool("DoAttackCombo", false);
                            MoveNext();
                        }
                    }
                }
                break;
            case IState.Commands.OnAction:
                {
                    // animation finished
                    if (_animationManager.IsPreviousAnimationFinished &&
                        _animationManager.GetCurrentNormalizedTime() > 0.9f)
                    {
                        MoveNext();
                    }
                    else if (_animationManager.GetBool("FinishCombo") == false)
                    {
                        if (_onCombo == false)
                        {
                            if (Input.GetKey(KeyCode.A))
                            {
                                _onCombo = true;
                                _comboCount++;
                                _animationManager.SetInt("ComboCount", _comboCount);
                                _animationManager.SetBool("DoAttackCombo", true);
                            }
                        }
                        else
                        {
                            _animationManager.DisableCombo();
                            Command = IState<T>.Commands.WaitUntilPrepared;
                        }
                    }
                }
                break;
            case IState.Commands.Finish:
                {
                    _animationManager.SetBool("FinishCombo", false);
                    MoveNext();
                }
                break;
            case IState.Commands.WaitUntilFinished:
                MoveNext();
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

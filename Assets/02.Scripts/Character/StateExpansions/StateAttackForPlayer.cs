using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttackForPlayer : StateBase
{
    private AnimationManagerBase _animationManager;
    private Movement _movement;
    private CharacterBase _character;
    private Rigidbody _rb;
    private bool _onCombo;
    private int _comboCount;
    public StateAttackForPlayer(StateMachineForPlayer.StateTypes stateType,
                                StateMachineBase machine)
        : base(stateType, machine)
    {
        _animationManager = machine.GetComponent<AnimationManagerBase>();
        _movement = machine.GetComponent<Movement>();
        _character = machine.GetComponent<CharacterBase>();
        _rb = machine.GetComponent<Rigidbody>();
    }

    public override bool Available => _animationManager.IsPreviousAnimationFinished &&
                                      (Machine.StateType == StateMachineForPlayer.StateTypes.Idle ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Move ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Jump ||
                                       Machine.StateType == StateMachineForPlayer.StateTypes.Fall);   

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

    public override dynamic Update()
    {
        dynamic nextStateType = StateType;

        switch (Command)
        {
            case Commands.Idle:
                break;
            case Commands.Prepare:
                {
                    _comboCount = 0;
                    _animationManager.SetInt("ComboCount", _comboCount);
                    _animationManager.SetBool("WeaponEquiped", _character.WeaponEquiped);
                    _animationManager.SetBool("DoAttack", true);
                    MoveNext();
                }
                break;
            case Commands.WaitUntilPrepared:
                {
                    if (_animationManager.IsPreviousAnimationFinished)
                    {
                        MoveNext();   
                    }
                }
                break;
            case Commands.Casting:
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
            case Commands.OnAction:
                {
                    // animation finished
                    if (_animationManager.GetCurrentNormalizedTime() > 0.9f)
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
                            Command = Commands.WaitUntilPrepared;
                        }
                    }
                }
                break;
            case Commands.Finish:
                {
                    _animationManager.SetBool("FinishCombo", false);
                    MoveNext();
                }
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
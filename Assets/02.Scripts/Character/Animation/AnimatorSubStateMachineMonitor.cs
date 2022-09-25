using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSubStateMachineMonitor : StateMachineBehaviour
{
    [SerializeField] string _boolParameter;
    public event Action<string> OnEnter;
    public event Action<string> OnExit;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {    
        animator.SetBool(_boolParameter, true);
        OnEnter?.Invoke(_boolParameter);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.SetBool(_boolParameter, false);
        OnExit?.Invoke(_boolParameter);
    }
}

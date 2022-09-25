using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSubStateMonitor : StateMachineBehaviour
{
    [SerializeField] string _boolParameter;
    public event Action<int> OnEnter;
    public event Action<int> OnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (string.IsNullOrEmpty(_boolParameter) == false)
            animator.SetBool(_boolParameter, true);

        OnEnter?.Invoke(stateInfo.fullPathHash);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (string.IsNullOrEmpty(_boolParameter) == false)
            animator.SetBool(_boolParameter, false);

        OnExit?.Invoke(stateInfo.fullPathHash);
    }
}

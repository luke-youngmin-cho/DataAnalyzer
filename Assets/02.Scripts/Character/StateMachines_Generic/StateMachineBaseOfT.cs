using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for generic enum state machine 
/// </summary>
/// <typeparam name="T">enum state types</typeparam>
public abstract class StateMachineBase<T> : MonoBehaviour where T : Enum
{
    public bool IsReady;
    public T StateType;
    public IState<T>.Commands CurrentCommand => Current.Command;
    protected IState<T> Current;
    protected Dictionary<T, IState<T>> States = new Dictionary<T, IState<T>>();    
    protected Dictionary<T, T> ExecutionMasks = new Dictionary<T, T>();
    protected Dictionary<T, T[]> TransitionPairs = new Dictionary<T, T[]>();    
    protected bool IsExecutionMaskInitialized;
    protected bool IsTransitionPairsInitialized;


    //==============================================================================
    //****************************** Public Methods ********************************
    //==============================================================================

    public virtual void ChangeState(T newStateType)
    {
        if (EqualityComparer<T>.Default.Equals(StateType, newStateType))
        {
            if (Current.IsFinished)
                newStateType = default(T);
            else
                return;
        }

        if (States[newStateType].Available == false)
            return;

        Debug.Log($"State changed {StateType} -> {newStateType}");
        Current.Deactive();
        States[newStateType].Active();
        Current = States[newStateType];
        StateType = newStateType;
    }


    //==============================================================================
    //**************************** Protected Methods *******************************
    //==============================================================================

    protected virtual void Awake()
    {
        StartCoroutine(E_Init());    
    }

    protected virtual IEnumerator E_Init()
    {
        InitExecutioneMasks();
        InitTransitionPairs();
        yield return new WaitUntil(() => IsExecutionMaskInitialized && 
                                         IsTransitionPairsInitialized);        
        RefreshStates();
        Init();
        IsReady = true;
    }

    protected virtual void Init()
    {
        // initialzie fields / properties
    }

    protected abstract void InitExecutioneMasks();

    protected abstract void InitTransitionPairs();

    protected abstract void RefreshStates();

    protected virtual void UpdateState()
    {
        if (IsReady == false)
            return;

        ChangeState(Current.Update());
    }

    protected virtual void FixedUpdateState()
    {
        Current.FixedUpdate();
    }

    protected void AddState(T stateType)
    {
        if (States.ContainsKey(stateType))
            return;

        string stateName = Convert.ToString(stateType);
        string typeName = "State" + stateName + "OfT";
        Debug.Log($"Adding state ... {typeName}");
        Type type = Type.GetType(typeName);
        if (type != null)
        {
            ConstructorInfo constructorInfo =
                type.GetConstructor(new[] 
                { 
                    typeof(T),
                    typeof(T[]),
                    typeof(T),
                    typeof(StateMachineBase) 
                });

            StateBase<T> state =
                constructorInfo.Invoke(new object[]
                {
                    stateType,
                    TransitionPairs[StateType],
                    ExecutionMasks[stateType],
                    this
                }) as StateBase<T>;

            States.Add(stateType, state);
            Debug.Log($"{stateType} is added");
        }
    }
}

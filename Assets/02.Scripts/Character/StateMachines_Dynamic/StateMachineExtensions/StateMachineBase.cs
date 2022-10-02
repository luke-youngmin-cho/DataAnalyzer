using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineBase : MonoBehaviour
{
    public bool IsReady;
    public dynamic StateType;
    protected Dictionary<dynamic, IState> States = new Dictionary<dynamic, IState>();
    protected IState Current;
    public IState.Commands CurrentCommand => Current.Command;

    public virtual void ChangeState(dynamic newStateType)
    {
        if (StateType == newStateType)
            return;

        if (States[newStateType].Available == false)
            return;

        Debug.Log($"State changed {StateType} -> {newStateType}");
        Current.Deactive();
        States[newStateType].Active();
        Current = States[newStateType];
        StateType = newStateType;
    }

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

    protected void AddState(dynamic stateType)
    {
        if (States.ContainsKey(stateType))
            return;

        string stateName = Convert.ToString(stateType);
        string typeName = "State" + stateName + this.GetType().Name.Replace("StateMachine", "");
        Debug.Log($"Adding state ... {typeName}");
        Type type = Type.GetType(typeName);
        if (type != null)
        {

            var constructors = type.GetConstructors();
            Debug.Log(constructors.Length);
            //ConstructorInfo constructorInfo =
            //    type.GetConstructor(new[] 
            //    { 
            //        typeof(dynamic), 
            //        typeof(StateMachineBase) 
            //    });
            
            StateBase state =
                constructors[0].Invoke(new object[] 
                {
                    stateType, 
                    this 
                }) as StateBase;

            States.Add(stateType, state);
            Debug.Log($"{stateType} is added");
        }
    }

    #region For debugging
    public IState GetCurrentState(out dynamic stateType)
    {
        stateType = StateType;
        return Current;
    }
    #endregion
}

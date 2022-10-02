using System;
using System.Collections;

/// <summary>
/// Base for state machine's sub state
/// </summary>
public abstract class StateBase : IState
{
    public bool IsBusy
    {
        get => (Command > IState.Commands.Idle || Command < IState.Commands.Finished) ? true : false;
    }

    public bool IsFinished
    {
        get => Command == IState.Commands.Finished;
    }

    public bool IsError
    {
        get => Command >= IState.Commands.Error;
    }

    public IState.Commands Command { get; protected set; }
    protected dynamic StateType = null;
    protected StateMachineBase Machine;
    

    public StateBase(dynamic stateType, StateMachineBase machine)
    {
        StateType = stateType;
        Machine = machine;
    }

    
    public abstract bool Available { get; }
    public virtual void Active() => Command = IState.Commands.Prepare;
    public virtual void Deactive() => Command = IState.Commands.Idle;
    public virtual dynamic Update()
    {
        dynamic nextStateType = StateType;
        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                MoveNext();
                break;
            case IState.Commands.WaitUntilPrepared:
                MoveNext();
                break;
            case IState.Commands.Casting:
                MoveNext();
                break;
            case IState.Commands.OnAction:
                MoveNext();
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
                MoveNext();
                break;
            case IState.Commands.WaitUntilErrorCleared:
                break;
            default:
                break;
        }
        return nextStateType;
    }
    public virtual void FixedUpdate()
    {
        switch (Command)
        {
            case IState.Commands.Idle:
                break;
            case IState.Commands.Prepare:
                break;
            case IState.Commands.Casting:
                break;
            case IState.Commands.OnAction:
                break;
            case IState.Commands.Finish:
                break;
            case IState.Commands.WaitUntilFinished:
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
    }
    public virtual void MoveNext() => Command++;

}

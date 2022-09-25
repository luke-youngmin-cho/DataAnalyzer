using System;
using System.Collections;

/// <summary>
/// Base for state machine's sub state
/// </summary>
public abstract class StateBase
{
    public bool IsBusy
    {
        get => (Command > Commands.Idle || Command < Commands.Finished) ? true : false;
    }

    public bool IsFinished
    {
        get => Command == Commands.Finished;
    }

    public bool IsError
    {
        get => Command >= Commands.Error;
    }

    public enum Commands
    {
        Idle,
        Prepare,
        WaitUntilPrepared,
        Casting,
        OnAction,
        Finish,
        WaitUntilFinished,
        Finished,
        Error,
        WaitUntilErrorCleared
    }

    public Commands Command { get; protected set; }
    protected dynamic StateType = null;
    protected StateMachineBase Machine;
    

    public StateBase(dynamic stateType, StateMachineBase machine)
    {
        StateType = stateType;
        Machine = machine;
    }

    
    public abstract bool Available { get; }
    public virtual void Active() => Command = Commands.Prepare;
    public virtual void Deactive() => Command = Commands.Idle;
    public virtual dynamic Update()
    {
        dynamic nextStateType = StateType;
        switch (Command)
        {
            case Commands.Idle:
                break;
            case Commands.Prepare:
                MoveNext();
                break;
            case Commands.WaitUntilPrepared:
                MoveNext();
                break;
            case Commands.Casting:
                MoveNext();
                break;
            case Commands.OnAction:
                MoveNext();
                break;
            case Commands.Finish:
                MoveNext();
                break;
            case Commands.WaitUntilFinished:
                MoveNext();
                break;
            case Commands.Finished:
                break;
            case Commands.Error:
                MoveNext();
                break;
            case Commands.WaitUntilErrorCleared:
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
            case Commands.Idle:
                break;
            case Commands.Prepare:
                break;
            case Commands.Casting:
                break;
            case Commands.OnAction:
                break;
            case Commands.Finish:
                break;
            case Commands.WaitUntilFinished:
                break;
            case Commands.Finished:
                break;
            case Commands.Error:
                break;
            case Commands.WaitUntilErrorCleared:
                break;
            default:
                break;
        }
    }
    public virtual void MoveNext() => Command++;

}

using System;

/// <summary>
/// Base for generic state machine's sub state
/// </summary>
public abstract class StateBase<T> : IState<T> where T : Enum
{
    public bool IsBusy
    {
        get => Command > IState<T>.Commands.Idle || 
               Command < IState<T>.Commands.Finished;
    }

    public bool IsFinished
    {
        get => Command == IState<T>.Commands.Finished;
    }

    public bool IsError
    {
        get => Command >= IState<T>.Commands.Error;
    }

    public IState<T>.Commands Command { get; protected set; }
    protected T StateType;
    protected T[] NextTargets;
    protected Func<bool>[] FinishConditions;
    protected T CanExecuteConditionMask;
    protected StateMachineBase<T> Machine;
    protected AnimationManagerBase AnimationManager;


    public StateBase(T stateType, T[] nextTargets, T canExecuteConditionMask, StateMachineBase<T> machine)
    {
        StateType = stateType;
        NextTargets = nextTargets;
        CanExecuteConditionMask = canExecuteConditionMask;
        Machine = machine;
        AnimationManager = Machine.GetComponent<AnimationManagerBase>();
    }

    /// <summary>
    /// This methods must be called in constructor ! 
    /// override this when state need multiple conditions so can implement various transitions.
    /// </summary>
    protected virtual void DefineFinishConditions()
    {
        FinishConditions = new Func<bool>[1];
        FinishConditions[0] = () => true;
    }

    public virtual bool Available => AnimationManager.IsPreviousAnimationFinished &&
                                     Machine.StateType.HasFlag(CanExecuteConditionMask);
    public virtual void Active() => Command = IState<T>.Commands.Prepare;
    public virtual void Deactive() => Command = IState<T>.Commands.Idle;
    public virtual void MoveNext() => Command++;

    public virtual T Update()
    {
        T nextStateType = StateType;
        switch (Command)
        {
            case IState<T>.Commands.Idle:
                break;
            case IState<T>.Commands.Prepare:
                MoveNext();
                break;
            case IState<T>.Commands.WaitUntilPrepared:
                MoveNext();
                break;
            case IState<T>.Commands.Casting:
                MoveNext();
                break;
            case IState<T>.Commands.OnAction:
                MoveNext();
                break;
            case IState<T>.Commands.Finish:
                MoveNext();
                break;
            case IState<T>.Commands.WaitUntilFinished:
                MoveNext();
                break;
            case IState<T>.Commands.Finished:
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
            case IState<T>.Commands.Error:
                MoveNext();
                break;
            case IState<T>.Commands.WaitUntilErrorCleared:
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
            case IState<T>.Commands.Idle:
                break;
            case IState<T>.Commands.Prepare:
                break;
            case IState<T>.Commands.Casting:
                break;
            case IState<T>.Commands.OnAction:
                break;
            case IState<T>.Commands.Finish:
                break;
            case IState<T>.Commands.WaitUntilFinished:
                break;
            case IState<T>.Commands.Finished:
                break;
            case IState<T>.Commands.Error:
                break;
            case IState<T>.Commands.WaitUntilErrorCleared:
                break;
            default:
                break;
        }
    }
}

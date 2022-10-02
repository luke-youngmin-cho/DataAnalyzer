using System.Windows.Input;
using System;

public interface IState
{
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
    bool IsBusy { get; }
    bool IsFinished { get; }
    bool IsError { get; }
    bool Available { get; }
    Commands Command { get; }
    void Active();
    void Deactive();
    dynamic Update();
    void FixedUpdate();
    void MoveNext();
}
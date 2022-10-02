using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Flags]
public enum PlayerStateTypes
{
    Idle = 0 << 0,
    Move = 1 << 0,
    Jump = 1 << 1,
    Fall = 1 << 2,
    Land = 1 << 3,
    Attack = 1 << 4,
    Crouch = 1 << 5,
    Hurt = 1 << 6,
    Die = 1 << 7,
    ALL = ~Idle
}

public class StateMachineOfPlayer : StateMachineBase<PlayerStateTypes>
{
    // todo -> Load below data from .CVS
    protected override void InitExecutioneMasks()
    {
        // previous states conditions to execute next state
        ExecutionMasks.Add(PlayerStateTypes.Idle, PlayerStateTypes.ALL);
        ExecutionMasks.Add(PlayerStateTypes.Move, PlayerStateTypes.ALL);
        ExecutionMasks.Add(PlayerStateTypes.Jump, PlayerStateTypes.Idle | PlayerStateTypes.Move | PlayerStateTypes.Crouch);
        ExecutionMasks.Add(PlayerStateTypes.Fall, PlayerStateTypes.Idle | PlayerStateTypes.Move | PlayerStateTypes.Jump | PlayerStateTypes.Crouch);
        ExecutionMasks.Add(PlayerStateTypes.Land, PlayerStateTypes.Fall);
        ExecutionMasks.Add(PlayerStateTypes.Attack, PlayerStateTypes.Idle | PlayerStateTypes.Move);
        ExecutionMasks.Add(PlayerStateTypes.Crouch, PlayerStateTypes.Idle | PlayerStateTypes.Move);
        ExecutionMasks.Add(PlayerStateTypes.Hurt, PlayerStateTypes.Idle | PlayerStateTypes.Move | PlayerStateTypes.Jump | PlayerStateTypes.Fall | PlayerStateTypes.Land);
        ExecutionMasks.Add(PlayerStateTypes.Die, PlayerStateTypes.ALL);

        IsExecutionMaskInitialized = true;
    }

    // todo -> Load below data from .CVS
    protected override void InitTransitionPairs()
    {
        // pairs to translate state when current state is finished
        TransitionPairs.Add(PlayerStateTypes.Idle, new[] { PlayerStateTypes.Idle });
        TransitionPairs.Add(PlayerStateTypes.Move, new[] { PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Jump, new[] { PlayerStateTypes.Fall });
        TransitionPairs.Add(PlayerStateTypes.Fall, new[] { PlayerStateTypes.Land, PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Land, new[] { PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Attack, new[] { PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Crouch, new[] { PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Hurt, new[] { PlayerStateTypes.Move });
        TransitionPairs.Add(PlayerStateTypes.Die, new[] { PlayerStateTypes.Idle });

        IsTransitionPairsInitialized = true;
    }

    protected override void RefreshStates()
    {
        var values = Enum.GetValues(typeof(PlayerStateTypes));
        foreach (PlayerStateTypes value in values)
            AddState(value);
        States[PlayerStateTypes.Move].Active();
        Current = States[PlayerStateTypes.Move];
        StateType = PlayerStateTypes.Move;
    }
}

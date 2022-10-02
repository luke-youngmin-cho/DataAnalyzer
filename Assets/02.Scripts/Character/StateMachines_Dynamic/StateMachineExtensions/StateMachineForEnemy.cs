using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineForEnemy : StateMachineBase
{
    public enum StateTypes
    {
        Idle,
        Move,
        Jump,
        Fall,
        Land,
        Attack,
        Crouch,
        Hurt,
        Die
    }

    protected override void RefreshStates()
    {
        throw new System.NotImplementedException();
    }
}

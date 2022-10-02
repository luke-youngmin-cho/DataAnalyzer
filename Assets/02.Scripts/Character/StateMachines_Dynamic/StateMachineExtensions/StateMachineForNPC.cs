using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StateMachineForNPC : StateMachineBase
{
    public enum StateTypes
    {
        Idle,
        Move,
        Talk1,
        Talk2,
        Talk3,
        Pose1,
        Pose2,
        Pose3,
        Pose4,
        Pose5
    }

    protected override void RefreshStates()
    {
        throw new NotImplementedException();
    }
}
using UnityEngine;
public class ShortCutSettings
{
    

    public static void SetUpDefaultStateMachineSettings(StateMachineBase stateMachine)
    {
        InputHandler.SetAction(
            KeyCode.LeftAlt,
            () => stateMachine.ChangeState(StateMachineForPlayer.StateTypes.Jump)
        );

        InputHandler.SetAction(
            KeyCode.A,
            () => stateMachine.ChangeState(StateMachineForPlayer.StateTypes.Attack)
        );

        InputHandler.SetAction(
            KeyCode.C,
            () => stateMachine.ChangeState(StateMachineForPlayer.StateTypes.Crouch)
        );
    }
}
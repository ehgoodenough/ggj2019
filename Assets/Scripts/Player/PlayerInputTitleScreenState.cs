using UnityEngine;
using System.Collections;

public class PlayerInputTitleScreenState : PlayerInputState
{
    protected override void DoAwake()
    {
        // Debug.Log("PlayerInputTitleScreenState.DoAwake()");
        base.DoAwake();
    }

    public override void DoUpdate()
    {
        if (gameStateMachine.currentState.GetType() == typeof(GameStateTitleScreen))
        {
            if (Input.GetKeyDown(KeyCode.Return) || (player.GetButtonDown("Interact") && !Input.GetMouseButton(0)))
            {
                Debug.Log("StartGame");
                EventBus.PublishEvent(new StartGameEvent());
                this.stateMachine.ChangeState(defaultState);
            }
        }
    }
}

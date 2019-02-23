using UnityEngine;
using System.Collections;

public class PlayerInputTitleScreenState : PlayerInputState
{
    protected override void DoAwake()
    {
        // Debug.Log("PlayerInputTitleScreenState.DoAwake()");
        base.DoAwake();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void DoUpdate()
    {
        // Now that this has been refactored, we probably do not need to check the game state like this
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

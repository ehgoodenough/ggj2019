using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInputPauseMenuState : PlayerInputState
{
    enum PauseOption
    {
        // Restart = 0,
        Resume = 0,
        Quit = 1
    }

    private PauseOption currentPauseOption = PauseOption.Resume;

    protected override void DoAwake()
    {
        // Debug.Log("PlayerInputPauseMenuState.DoAwake()");
        base.DoAwake();
    }

    protected override void DoEnter()
    {
        Pause();
    } 

    public override void DoUpdate()
    {
        // For now, restart is a cheat executed from the pause menu
        if (Input.GetKey(KeyCode.BackQuote) && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
            return;
        }

        // Hitting Esc resumes the game
        if (Input.GetKeyUp(KeyCode.Escape) || player.GetButtonUp("Pause") || player.GetButtonUp("Back"))
        {
            Resume();
        }
        // Hitting enter executes the current pause option
        else if (Input.GetKeyUp(KeyCode.Return) || (player.GetButtonUp("Interact") && !Input.GetMouseButton(0)))
        {
            // Debug.Log("Return");
            switch (currentPauseOption)
            {
                // case PauseOption.Restart:
                //     Restart();
                //    break;
                case PauseOption.Resume:
                    Resume();
                    break;
                case PauseOption.Quit:
                    Quit();
                    break;
            }
        }
        // Hitting the arrow keys (including WASD) scrolls selection through Pause Menu options
        else
        {
            bool selectionSwitched = false;
            if (Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKey(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) ||
                player.GetButtonDown("Pause Menu Up"))
            {
                currentPauseOption = (PauseOption)(((int)currentPauseOption - 1 + 2) % 2); // + 3) % 3);
                // Debug.Log("Current Pause Option: " + currentPauseOption);
                selectionSwitched = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S) ||
                        Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow) ||
                        player.GetButtonDown("Pause Menu Down"))
            {
                currentPauseOption = (PauseOption)(((int)currentPauseOption + 1) % 2); // % 3);
                // Debug.Log("Current Pause Option: " + currentPauseOption);
                selectionSwitched = true;
            }

            if (selectionSwitched)
            {
                switch (currentPauseOption)
                {
                    // case PauseOption.Restart:
                    //     EventBus.PublishEvent(new SwitchFocusToRestartOptionEvent());
                    //     break;
                    case PauseOption.Resume:
                        EventBus.PublishEvent(new SwitchFocusToResumeOptionEvent());
                        break;
                    case PauseOption.Quit:
                        EventBus.PublishEvent(new SwitchFocusToQuitOptionEvent());
                        break;
                }
            }
        }
    }

    private void Pause()
    {
        // Debug.Log("Pause Game")
        EventBus.PublishEvent(new PauseMenuEngagedEvent());

        currentPauseOption = PauseOption.Resume;
        // Debug.Log("Current Pause Option: " + currentPauseOption);
        EventBus.PublishEvent(new SwitchFocusToResumeOptionEvent());

        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            rb.Sleep();
        }
    }

    private void Resume()
    {
        // Debug.Log("Resume Game");
        EventBus.PublishEvent(new PauseMenuDisengagedEvent());
        
        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            rb.WakeUp();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        this.stateMachine.ChangeState(defaultState);
    }

    public void Restart()
    {
        Debug.Log("Restart Game");
        EventBus.PublishEvent(new PauseMenuDisengagedEvent());

        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            rb.WakeUp();
        }

        // TODO: Figure out all the things we're going to need to reset to handle at restart
        SceneManager.LoadScene("RobertTitleScreen"); // TODO: Listen for an event in game state machine to handle this
        gameStateMachine.ChangeState(gameStateMachine.GetState<GameStateTitleScreen>());
        this.stateMachine.ChangeState(titleScreenState);
    }

    private void Quit()
    {
        // Debug.Log("Quit Game");
        Application.Quit();
    }
}

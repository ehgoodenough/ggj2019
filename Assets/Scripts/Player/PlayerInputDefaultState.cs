using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerView))]
public class PlayerInputDefaultState : State
{
    private int playerId = 0; // The Rewired player id
    private Player player; // The Rewired Player

    private PlayerMovement movement;
    private PlayerView playerView;
    private InteractionDetector interactionDetector;
    private PickupHolder pickupHolder;

    private StateMachine gameStateMachine;

    private Photo photo;

    private bool canPause = false;
    private bool isPaused = false;

    enum PauseOption
    {
        // Restart = 0,
        Resume = 0,
        Quit = 1
    }
    private PauseOption currentPauseOption = PauseOption.Resume;

    protected override void DoAwake()
    {
        Debug.Log("PlayerInputStateBase.DoAwake()");

        player = ReInput.players.GetPlayer(playerId);

        movement = GetComponent<PlayerMovement>();
        playerView = GetComponent<PlayerView>();
        interactionDetector = GetComponentInChildren<InteractionDetector>();
        pickupHolder = GetComponentInChildren<PickupHolder>();

        photo = GetComponentInChildren<Photo>();

        gameStateMachine = FindObjectOfType<GameStateTitleScreen>().GetComponent<StateMachine>();

        EventBus.Subscribe<PhotoLoweredAtStartEvent>(e => canPause = true);
        EventBus.Subscribe<ObjectiveCompletedCutsceneStartEvent>(e => canPause = false);
        EventBus.Subscribe<ObjectiveCompletedCutsceneEndEvent>(e => canPause = true);
        EventBus.Subscribe<PlayerHasWonEvent>(e => canPause = false);
    }

    protected override void DoEnter()
    {
        base.DoEnter();
        Debug.Log("PlayerInputDefaultState.DoEnter()");
    }

    public override void DoUpdate()
    {
        if (gameStateMachine.currentState.GetType() == typeof(GameStateTitleScreen))
        {
            if (Input.GetKeyDown(KeyCode.Return) || (player.GetButtonDown("Interact") && !Input.GetMouseButton(0)))
            {
                Debug.Log("StartGame");
                EventBus.PublishEvent(new StartGameEvent());
            }
        }
        else
        {
            // How much of this pause menu stuff can we offload into another script
            if (isPaused) // Game is currently in the pause screen
            {
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
            else if (!isPaused)
            {
                if (canPause && (Input.GetKeyUp(KeyCode.Escape) || player.GetButtonUp("Pause")))
                {
                    Pause();
                    return;
                }

                // Hold back tick, then press 'O' to cheat an objective item in front of you
                if (Input.GetKey(KeyCode.BackQuote) && Input.GetKeyDown(KeyCode.O))
                {
                    Debug.Log("Cheat Keys Hit");
                    ObjectivePickupable objectiveItem = GameObject.FindObjectOfType<ObjectivePickupable>();
                    if (objectiveItem)
                    {
                        Debug.Log("Objective Item Found");
                        if (Vector3.Distance(movement.transform.position, objectiveItem.transform.position) > 5f)
                        {
                            Debug.Log("Dropping " + objectiveItem + " in front of player.");
                            Vector3 playerForwardDirection = movement.transform.TransformDirection(Vector3.forward);
                            objectiveItem.transform.position = movement.transform.position + (playerForwardDirection * 3f) + Vector3.up;
                        }
                    }
                }

                if (Input.GetKey("space") || player.GetAxis("ShowPhoto") > 0)
                {
                    photo.ShowPhoto();
                }
                else
                {
                    photo.HidePhoto();
                }

                Interactable focusedInteractable = interactionDetector.GeInteractableInFocus(); // Note: the held item is never focused

                // Handle Movement & Looking
                if (focusedInteractable == null || !focusedInteractable.IsInteractionRestrictingMovement())
                {
                    float keyboardVertical = Input.GetAxisRaw("Vertical");
                    float keyboardHorizontal = Input.GetAxisRaw("Horizontal");

                    float forward = Mathf.Abs(keyboardVertical) > 0 ? keyboardVertical : player.GetAxis("Move Vertical");
                    float strafe = Mathf.Abs(keyboardHorizontal) > 0 ? keyboardHorizontal : player.GetAxis("Move Horizontal");

                    Vector3 moveVector = Vector3.forward * forward + Vector3.right * strafe;
                    moveVector = moveVector.sqrMagnitude > 1f ? moveVector.normalized : moveVector;
                    bool isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || player.GetAxis("Run") > 0f);
                    movement.Move(moveVector, isRunning);
                }

                if (focusedInteractable == null || !focusedInteractable.IsInteractionRestrictingView())
                {
                    float lookX = player.GetAxis("Look Horizontal"); // Input.GetAxis("Mouse X");
                    float lookY = player.GetAxis("Look Vertical"); // Input.GetAxis("Mouse Y");

                    Vector2 lookVector = Vector2.up * lookY + Vector2.right * lookX;
                    playerView.Look(lookVector); // Looking along vertical axis only rotates the camera view
                    playerView.AddToYaw(lookX); // Looking along horizontal axis rotates the player
                }

                ///                                 IsInteracting == true               isInteracting == false
                /// focusedInteractable == null               N\A               |        attempt pick up / drop     
                ///                              -------------------------------|--------------------------------------
                /// focusedInteractable != null     handle multi interaction    |     begin interaction attempt

                // Handle Interactions
                if (focusedInteractable != null)
                {
                    // Handle Beginning of Interaction
                    if (!focusedInteractable.IsInteracting())
                    {
                        // Handle Picking Up item
                        Pickupable focusedPickupable = focusedInteractable.GetComponent<Pickupable>();
                        if (focusedPickupable != null && pickupHolder.GetHeldItem() == null)
                        {
                            // Possibly split out pick and and drop if we map these actions to different inputs
                            bool pickUp = player.GetButtonDown("Pickup/Drop"); // Input.GetMouseButtonDown(0);
                            if (pickUp) Debug.Log("Pick Up at " + Time.time);
                            if (pickUp)
                            {
                                pickupHolder.TryPickupOrDrop(); // This will try to pickup when held item is null
                            }
                        }
                        // Attempt beginning of iteraction
                        else
                        {
                            bool interact = player.GetButtonDown("Interact"); // Input.GetMouseButtonDown(0);
                            if (interact) Debug.Log("Interact at " + Time.time);
                            if (interact && focusedInteractable != null)
                            {
                                interactionDetector.PerformInteraction();
                            }
                        }
                    }
                    // Handle Multiple Input Interaction
                    else
                    {
                        // Do stuff here
                    }
                }
                // Drop item
                else
                {
                    // Possibly split out pick and and drop if we map these actions to different inputs
                    bool drop = player.GetButtonDown("Pickup/Drop"); // Input.GetMouseButtonDown(0);
                    if (drop) Debug.Log("Drop at " + Time.time);
                    if (drop)
                    {
                        pickupHolder.TryPickupOrDrop(); // This will drop the held item if not null
                    }
                }
            }
        }
    }

    // Can we extract this stuff into an independent script?
    private void Pause()
    {
        // Debug.Log("Pause Game");
        isPaused = true;
        EventBus.PublishEvent(new PauseMenuEngagedEvent());
        // pauseUI.gameObject.SetActive(true);
        currentPauseOption = PauseOption.Resume;
        EventBus.PublishEvent(new SwitchFocusToResumeOptionEvent());
        // Debug.Log("Current Pause Option: " + currentPauseOption);
        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            rb.Sleep();
        }
    }

    private void Resume()
    {
        // Debug.Log("Resume Game");
        isPaused = false;
        EventBus.PublishEvent(new PauseMenuDisengagedEvent());
        // pauseUI.gameObject.SetActive(false);
        currentPauseOption = PauseOption.Resume;
        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            rb.WakeUp();
        }
    }

    /*
    public void Restart()
    {
        // Debug.Log("Restart Game");
        // TODO: Figure out all the things we're going to need to reset to handle a restart
        SceneManager.LoadScene("RobertTitleScreen");
    }
    */

    private void Quit()
    {
        // Debug.Log("Quit Game");
        Application.Quit();
    }
}

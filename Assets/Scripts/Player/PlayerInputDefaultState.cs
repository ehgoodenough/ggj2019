﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerView))]
public class PlayerInputDefaultState : PlayerInputState
{
    public bool debugCheatsEnabled = false;

    private PlayerMovement movement;
    private PlayerView playerView;
    private InteractionDetector interactionDetector;
    private PickupHolder pickupHolder;
    private Photo photo;

    private bool canPause = false;
    
    protected override void DoAwake()
    {
        // Debug.Log("PlayerInputDefaultState.DoAwake()");
        base.DoAwake();

        movement = GetComponentInChildren<PlayerMovement>();
        playerView = GetComponentInChildren<PlayerView>();
        interactionDetector = GetComponentInChildren<InteractionDetector>();
        pickupHolder = GetComponentInChildren<PickupHolder>();
        photo = GetComponentInChildren<Photo>();

        // gameStateMachine = FindObjectOfType<GameStateTitleScreen>().GetComponent<StateMachine>();

        EventBus.Subscribe<EnterTitleScreenEvent>(e => canPause = false);
        EventBus.Subscribe<PhotoLoweredAtStartEvent>(e => canPause = true);
        EventBus.Subscribe<ObjectiveCompletedCutsceneStartEvent>(e => canPause = false);
        EventBus.Subscribe<ObjectiveCompletedCutsceneEndEvent>(e => canPause = true);
        EventBus.Subscribe<PlayerHasWonEvent>(e => canPause = false);
    }

    protected override void DoEnter()
    {
        base.DoEnter();
        // Debug.Log("PlayerInputDefaultState.DoEnter()");
    }

    public override void DoUpdate()
    {
        if (canPause && (Input.GetKeyUp(KeyCode.Escape) || player.GetButtonUp("Pause")))
        {
            this.stateMachine.ChangeState(pauseMenuState);
            return;
        }
        
        if (debugCheatsEnabled)
        {
            // Hold back tick, then press 'O' to cheat an objective item in front of you
            if (Input.GetKey(KeyCode.BackQuote) && Input.GetKeyDown(KeyCode.O))
            {
                // Debug.Log("Cheat Keys Hit");
                ObjectivePickupable objectiveItem = GameObject.FindObjectOfType<ObjectivePickupable>();
                if (objectiveItem)
                {
                    // Debug.Log("Objective Item Found");
                    if (Vector3.Distance(movement.transform.position, objectiveItem.transform.position) > 5f)
                    {
                        Debug.Log("Dropping " + objectiveItem + " in front of player.");
                        Vector3 playerForwardDirection = movement.transform.TransformDirection(Vector3.forward);
                        objectiveItem.transform.position = movement.transform.position + (playerForwardDirection * 3f) + Vector3.up;
                    }
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

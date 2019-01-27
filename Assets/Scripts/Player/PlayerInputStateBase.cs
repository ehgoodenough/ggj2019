using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerView))]
public class PlayerInputStateBase : State
{
    protected int playerId = 0; // The Rewired player id
    protected Player player; // The Rewired Player

    protected PlayerMovement movement;
    protected PlayerView playerView;
    protected InteractionDetector interactionDetector;
    protected PickupHolder pickupHolder;

    protected override void DoAwake()
    {
        player = ReInput.players.GetPlayer(playerId);

        movement = GetComponent<PlayerMovement>();
        playerView = GetComponent<PlayerView>();
        interactionDetector = GetComponentInChildren<InteractionDetector>();
        pickupHolder = GetComponentInChildren<PickupHolder>();
    }
}

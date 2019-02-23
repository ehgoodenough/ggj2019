using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public abstract class PlayerInputState : State
{
    protected PlayerInputState titleScreenState;
    protected PlayerInputState defaultState;

    protected int playerId = 0; // The Rewired player id
    protected Player player; // The Rewired Player

    protected StateMachine gameStateMachine;

    protected override void DoAwake()
    {
        titleScreenState = this.stateMachine.GetState<PlayerInputTitleScreenState>();
        defaultState = this.stateMachine.GetState<PlayerInputDefaultState>();

        player = player ?? ReInput.players.GetPlayer(playerId);

        gameStateMachine = gameStateMachine ?? FindObjectOfType<GameStateTitleScreen>().GetComponent<StateMachine>();
    }
}

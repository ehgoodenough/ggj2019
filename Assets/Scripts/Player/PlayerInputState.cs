using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public abstract class PlayerInputState : State
{
    protected PlayerInputState titleScreenState;
    protected PlayerInputState defaultState;
    protected PlayerInputState pauseMenuState;

    protected int playerId = 0; // The Rewired player id
    protected Player player; // The Rewired Player

    protected StateMachine gameStateMachine;

    protected override void DoAwake()
    {
        titleScreenState = this.stateMachine.GetState<PlayerInputTitleScreenState>();
        defaultState = this.stateMachine.GetState<PlayerInputDefaultState>();
        pauseMenuState = this.stateMachine.GetState<PlayerInputPauseMenuState>();

        player = player ?? ReInput.players.GetPlayer(playerId);

        gameStateMachine = gameStateMachine ?? FindObjectOfType<GameStateTitleScreen>().GetComponent<StateMachine>();
    }
}

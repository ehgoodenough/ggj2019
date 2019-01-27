﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateCity : State
{
    public Transform playerStart;
    public Transform dogStart;

    private GameStateTitleScreen titleState;
    private GameStateHome homeState; // string homeState = "Virginia";

    protected override void DoAwake()
    {
        // Debug.Log("City Player Start: " + playerStart.position);
        // Debug.Log("City Robot Dog Start: " + dogStart.position);

        titleState = stateMachine.GetState<GameStateTitleScreen>();
        homeState = stateMachine.GetState<GameStateHome>();
        EventBus.Subscribe<ReturnHomeEvent>(OnReturnHomeEvent);
    }

    protected override void DoStart()
    {
        // Do something on Start
    }

    protected override void DoEnter()
    {
        // Do stuff here that should always happen when you first enter the city game state
        EventBus.PublishEvent(new EnterCityEvent(this));
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when the city game state is active
    }

    protected override void DoExit()
    {
        // Do stuff here when you exit the city, but before you enter the home, or another place or game state
        EventBus.PublishEvent(new ExitCityEvent());
    }

    private void OnReturnHomeEvent(ReturnHomeEvent e)
    {
        // Debug.Log("GameStateCity.OnReturnHomeEvent");
        // Does the order of LoadScene() and ChangeState() matter?
        // SceneManager.LoadScene("HomeScene");
        // SceneManager.UnloadSceneAsync("RobertCityScene");
        SceneManager.LoadScene("RobertHomeScene");
        stateMachine.ChangeState(homeState);
    }
}

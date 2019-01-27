using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateCity : State
{
    GameStateHome homeState; // string homeState = "Virginia";

    protected override void DoAwake()
    {
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
        EventBus.PublishEvent(new EnterCityEvent());
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when the city game state is active
    }

    protected override void DoExit()
    {
        // Do stuff here when you exit the city, but before you enter the home, or another place or game state
    }

    private void OnReturnHomeEvent(ReturnHomeEvent e)
    {
        // Does the order of LoadScene() and ChangeState() matter?
        SceneManager.LoadScene("HomeScene");
        stateMachine.ChangeState(homeState);
    }
}

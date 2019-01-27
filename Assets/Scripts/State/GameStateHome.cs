using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateHome : State
{
    public Transform playerStart;
    public Transform dogStart;

    GameStateCity cityState; // string cityState = "Athens";

    protected override void DoAwake()
    {
        cityState = stateMachine.GetState<GameStateCity>();
        EventBus.Subscribe<LeaveHomeEvent>(OnLeaveHomeEvent);
    }

    protected override void DoStart()
    {
        // Do something on Start
    }

    protected override void DoEnter()
    {
        // Do something on the first frame when Entering the state
        EventBus.PublishEvent(new EnterCityEvent());
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when state is active
    }

    protected override void DoExit()
    {
        // Do something on the last frame while Exiting the state
    }

    private void OnLeaveHomeEvent(LeaveHomeEvent e)
    {
        // Does the order of LoadScene() and ChangeState() matter?
        // SceneManager.LoadScene("RobertCityScene");
        SceneManager.LoadScene("CityScene");
        stateMachine.ChangeState(cityState);
    }
}

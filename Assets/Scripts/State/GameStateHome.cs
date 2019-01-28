﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateHome : State
{
    public CanvasGroup sceneTransitionFade;
    public float fadeDuration = 1f;

    public Transform playerStart;
    public Transform dogStart;

    private GameStateTitleScreen titleState;
    private GameStateCity cityState; // string cityState = "Athens";

    protected override void DoAwake()
    {
        // Debug.Log("Home Player Start: " + playerStart.position);
        Debug.Log("Home Robot Dog Start: " + dogStart.position);

        titleState = stateMachine.GetState<GameStateTitleScreen>();
        cityState = stateMachine.GetState<GameStateCity>();
        EventBus.Subscribe<LeaveHomeEvent>(OnLeaveHomeEvent);
    }

    protected override void DoStart()
    {
        // Debug.Log("GameStateHome.DoStart()");

        /*
        if (playerObj == null)
        {
            playerObj = Instantiate(playerPrefab); // , playerStart.position, Quaternion.Euler(playerStart.position));
            Debug.Log("playerObj: " + playerObj);
            Debug.Log("Player Position: " + playerObj.transform.position);
        }
        */

        /*
        if (dogObj == null)
        {
            dogObj = Instantiate(dogPrefab);
        }
        */
    }

    protected override void DoEnter()
    {
        Debug.Log("GameStateHome.DoEnter()");
        StartCoroutine(FadeIn());

        EventBus.PublishEvent(new EnterHomeEvent(this));

        Debug.Log("start dat coroutine");
        FindObjectOfType<GlitchEffect>().enabled = false;
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when state is active
    }

    protected override void DoExit()
    {
        // Do something on the last frame while Exiting the state
        EventBus.PublishEvent(new ExitHomeEvent());
    }

    private void OnLeaveHomeEvent(LeaveHomeEvent e)
    {
        // Debug.Log("GameStateHome.OnLeaveHomeEvent");
        // Does the order of LoadScene() and ChangeState() matter?
        // SceneManager.UnloadSceneAsync("RobertHomeScene");
        SceneManager.LoadScene("RobertCityScene");
        // SceneManager.LoadScene("CityScene");
        stateMachine.ChangeState(cityState);
    }

    IEnumerator FadeIn()
    {
        sceneTransitionFade = GameObject.Find("SceneTransitionFade").GetComponent<CanvasGroup>();
        CanvasGroup otherCanvasGroup = GameObject.Find("FadePanel").GetComponent<CanvasGroup>();
        CanvasGroup powerDownTextCanvasGroup = GameObject.Find("PowerDownText").GetComponent<CanvasGroup>();

        powerDownTextCanvasGroup.alpha = 0;
        while (sceneTransitionFade.alpha > 0)
        {
            sceneTransitionFade.alpha -= Time.deltaTime / fadeDuration;
            if (otherCanvasGroup)
            {
                otherCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
                //Debug.Log("otherCanvasGroup alpha " + otherCanvasGroup.alpha);
            }
            yield return null;
        }
    }
}

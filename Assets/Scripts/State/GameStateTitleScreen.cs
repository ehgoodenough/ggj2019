using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateTitleScreen : State
{
    public GameObject playerPrefab;
    public float titleFadeDuration;

    private GameObject playerObj;

    private GameStateHome homeState;
    private GameStateCity cityState;

    private int titleScreenHandle;

    protected override void DoAwake()
    {
        homeState = stateMachine.GetState<GameStateHome>();
        cityState = stateMachine.GetState<GameStateCity>();

        EventBus.Subscribe<StartGameEvent>(OnGameStartEvent);
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    protected override void DoStart()
    {
        if (playerObj == null)
        {
            playerObj = Instantiate(playerPrefab); // , playerStart.position, Quaternion.Euler(playerStart.position));
            // Debug.Log("playerObj: " + playerObj);
            // Debug.Log("Player Position: " + playerObj.transform.position);
        }
    }

    private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject titleCanvasObj = GameObject.Find("TitleGroup");
        if (titleCanvasObj)
        {
            CanvasGroup titleCanvas = titleCanvasObj.GetComponent<CanvasGroup>();
            titleCanvas.alpha = 0;
            StartCoroutine(RevealCanvas(titleCanvas, 1.5f));
        }

        GameObject startPromptCanvasObj = GameObject.Find("StartPromptGroup");
        if (startPromptCanvasObj)
        {
            CanvasGroup startPromptCanvas = startPromptCanvasObj.GetComponent<CanvasGroup>();
            startPromptCanvas.alpha = 0;
            StartCoroutine(RevealCanvas(startPromptCanvas, 5f));
        }

        // Not the ideal way to do this, but it'll serve for now
        if (titleCanvasObj && startPromptCanvasObj)
        {
            EventBus.PublishEvent(new TitleScreenLoadedEvent());
        }
    }

    protected override void DoEnter()
    {
        // Do stuff here that should always happen when you first enter the title screen
        // Debug.Log("GameStateTitleScreen.DoEnter()");
        EventBus.PublishEvent(new EnterTitleScreenEvent(this));
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when the title screen game state is active
    }

    protected override void DoExit()
    {
        // Do stuff here when exiting the title screen but before entering another game state
        EventBus.PublishEvent(new ExitTitleScreenEvent());
    }

    IEnumerator RevealCanvas(CanvasGroup canvas, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        while (canvas && canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime / titleFadeDuration;
            yield return null;
            while (canvas == null)
            {
                yield return null;
            }
        }
    }

    private void OnGameStartEvent(StartGameEvent e)
    {
        // Note that this event should be for starting the game from scratch, not necessarily loading a saved game
        DoorToCity.enteringFromTitleScreen = true; // This ensures that the door closing sound does not play during boot up sequence
        SceneManager.LoadScene("RobertHomeScene");
        stateMachine.ChangeState(homeState);
    }
}

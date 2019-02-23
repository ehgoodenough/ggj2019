using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateTitleScreen : State
{
    public GameObject playerPrefab;
    public float titleFadeDuration;
    public CanvasGroup titleCanvas;
    public CanvasGroup startPromptCanvas;

    private GameObject playerObj;

    private GameStateHome homeState;
    private GameStateCity cityState;

    protected override void DoAwake()
    {
        homeState = stateMachine.GetState<GameStateHome>();
        cityState = stateMachine.GetState<GameStateCity>();

        EventBus.Subscribe<StartGameEvent>(OnGameStartEvent);
    }

    protected override void DoStart()
    {
        if (playerObj == null)
        {
            playerObj = Instantiate(playerPrefab); // , playerStart.position, Quaternion.Euler(playerStart.position));
            // Debug.Log("playerObj: " + playerObj);
            // Debug.Log("Player Position: " + playerObj.transform.position);
        }

        // EventBus.PublishEvent(new TitleScreenStartEvent(playerObj));
        titleCanvas.alpha = 0;
        StartCoroutine(RevealCanvas(titleCanvas, 2f));

        startPromptCanvas.alpha = 0;
        StartCoroutine(RevealCanvas(startPromptCanvas, 5.5f));
    }

    protected override void DoEnter()
    {
        // Do stuff here that should always happen when you first enter the title screen
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
        SceneManager.LoadScene("RobertHomeScene");
        stateMachine.ChangeState(homeState);
    }
}

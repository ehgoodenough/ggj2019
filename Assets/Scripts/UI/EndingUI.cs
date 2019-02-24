using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingUI : MonoBehaviour
{
    public CanvasGroup endingGroup;
    public CanvasGroup textBackgroundGroup;
    public CanvasGroup friendQuestionGroup;
    public CanvasGroup welcomeHomeGroup;
    public CanvasGroup whiteBackgroundGroup;
    public CanvasGroup blackBackgroundGroup;
    public CanvasGroup thanksForPlayingGroup;
    public CanvasGroup creditsGroup;
    public CanvasGroup titleFollowQuitGroup;

    private void Awake()
    {
        // Debug.Log("EndingUI.Awake()");
        Initialize();

        EventBus.Subscribe<FriendFullyAssembledEvent>(OnFriendFullyAssembled);
        EventBus.Subscribe<TitleScreenLoadedEvent>(e => Initialize());
    }

    private void Initialize()
    {
        // Debug.Log("Initialize Ending UI");
        endingGroup.alpha = 0f;
        textBackgroundGroup.alpha = 0f;
        friendQuestionGroup.alpha = 0f;
        welcomeHomeGroup.alpha = 0f;
        whiteBackgroundGroup.alpha = 0f;
        blackBackgroundGroup.alpha = 0f;
        thanksForPlayingGroup.alpha = 0f;
        creditsGroup.alpha = 0f;
        titleFollowQuitGroup.alpha = 0f;
    }

    private void OnFriendFullyAssembled(FriendFullyAssembledEvent e)
    // private void OnFriendFullyAssembled(PhotoLoweredAtStartEvent e) // For testing only
    {
        Debug.Log("EndingUI.OnFriendFullyAssembled()");
        StartCoroutine(PlayEndingSequence());
    }

    IEnumerator PlayEndingSequence()
    {
        endingGroup.alpha = 1f;

        Debug.Log("Fade in 'Friend?'");
        StartCoroutine(FadeInFadeOut(textBackgroundGroup, 1.25f, 1.5f, 2.5f, 1.25f));
        StartCoroutine(FadeInFadeOut(friendQuestionGroup, 1.25f, 1.5f, 2.5f, 1.25f));

        yield return new WaitForSeconds(8f);

        Debug.Log("Fade in 'Welcome Home, Friend!'");
        StartCoroutine(FadeInFadeOut(textBackgroundGroup, 0.75f, 1.75f, 3f, 1.25f));
        StartCoroutine(FadeInFadeOut(welcomeHomeGroup, 0.75f, 1.75f, 3f, 1.25f));

        yield return new WaitForSeconds(7f);

        Debug.Log("Fade in White Background");
        StartCoroutine(FadeInFadeOut(whiteBackgroundGroup, 0.5f, 1f, 1.5f, 1f));

        yield return new WaitForSeconds(2f);

        blackBackgroundGroup.alpha = 1f;

        Debug.Log("Fade in 'Thanks For Playing!'");
        StartCoroutine(FadeInFadeOut(thanksForPlayingGroup, 0.75f, 1.5f, 2.5f, 1.25f));

        yield return new WaitForSeconds(7f);

        // Make this last longer
        Debug.Log("Fade in Credits & Special Thanks");
        StartCoroutine(FadeInFadeOut(creditsGroup, 0.75f, 1.5f, 9.5f, 1.25f));

        yield return new WaitForSeconds(12f);

        Debug.Log("Fade in Title, Follow, and Quit");
        StartCoroutine(FadeIn(titleFollowQuitGroup, 0.75f, 1.5f));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        Application.Quit();
    }

    IEnumerator FadeInFadeOut(CanvasGroup canvasGroup, float initialDelay, float fadeInDuration, float holdTime, float fadeOutDuration)
    {
        if (canvasGroup)
        {
            yield return new WaitForSeconds(initialDelay);

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime / fadeInDuration;
                yield return null;
            }

            yield return new WaitForSeconds(holdTime);

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeOutDuration;
                yield return null;
            }
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float delay = 0f, float fadeInDuration = 1f)
    {
        if (canvasGroup)
        {
            yield return new WaitForSeconds(delay);

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / fadeInDuration;
                yield return null;
            }
        }
    }
}

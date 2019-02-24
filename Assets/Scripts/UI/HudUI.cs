using UnityEngine;
using System.Collections;

public class HudUI : MonoBehaviour
{
    public float startFadeDelay = 0.5f;
    public float canvasGroupFadeDuration = 0.5f;
    public CanvasGroup partsCanvasGroup;
    public CanvasGroup batteryCanvasGroup;

    private void Awake()
    {
        Initialize();
        EventBus.Subscribe<ExitTitleScreenEvent>(OnExitTitleScreenEvent);
        EventBus.Subscribe<TitleScreenLoadedEvent>(e => Initialize());
    }

    private void Initialize()
    {
        // Debug.Log("HudUI.Initialize()");
        if (partsCanvasGroup) partsCanvasGroup.alpha = 0f;
        if (batteryCanvasGroup) batteryCanvasGroup.alpha = 0f;
    }

    private void OnExitTitleScreenEvent(ExitTitleScreenEvent e)
    {
        // StartCoroutine(RevealCanvasGroup(partsCanvasGroup));
        StartCoroutine(RevealCanvasGroup(batteryCanvasGroup));
    }

    IEnumerator RevealCanvasGroup(CanvasGroup canvasGroup, float delay = 0f)
    {
        if (!canvasGroup) yield return null;

        yield return new WaitForSeconds(delay);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / canvasGroupFadeDuration;
            yield return null;
        }
    }
}

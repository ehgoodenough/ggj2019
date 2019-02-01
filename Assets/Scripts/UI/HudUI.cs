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
        if (partsCanvasGroup) partsCanvasGroup.alpha = 0f;
        if (batteryCanvasGroup) partsCanvasGroup.alpha = 0f;
        EventBus.Subscribe<ExitTitleScreenEvent>(OnExitTitleScreenEvent);
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
            while (canvasGroup == null)
            {
                yield return null;
            }
        }
    }
}

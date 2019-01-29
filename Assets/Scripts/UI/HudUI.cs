using UnityEngine;
using System.Collections;

public class HudUI : MonoBehaviour
{
    public float startFadeDelay = 0.5f;
    public float canvasGroupFadeDuration = 0.5f;
    public CanvasGroup defaultCanvasGroup;

    private void Awake()
    {
        if (defaultCanvasGroup) defaultCanvasGroup.alpha = 0f;
        EventBus.Subscribe<ExitTitleScreenEvent>(OnExitTitleScreenEvent);
    }

    private void OnExitTitleScreenEvent(ExitTitleScreenEvent e)
    {
        StartCoroutine(RevealCanvasGroup(defaultCanvasGroup));
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

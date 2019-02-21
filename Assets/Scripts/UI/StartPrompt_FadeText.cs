using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPrompt_FadeText : MonoBehaviour
{
    public float titleFadeDuration;
    public CanvasGroup startPromptCanvas;


    void Awake()
    {
        // EventBus.PublishEvent(new TitleScreenStartEvent(playerObj));
        startPromptCanvas.alpha = 0;
        StartCoroutine(RevealCanvas(startPromptCanvas, 5.5f));
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
        StartCoroutine(FadeOut(startPromptCanvas, 2f));
    }

        IEnumerator FadeOut(CanvasGroup canvas, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        while (canvas && canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / titleFadeDuration;
            yield return null;
            while (canvas == null)
            {
                yield return null;
            }
        }
        StartCoroutine(RevealCanvas(startPromptCanvas, 0f));
    }
}

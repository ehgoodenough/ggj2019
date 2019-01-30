using UnityEngine;
using System.Collections;

public class ShowPhotoPromptUI : MonoBehaviour
{
    private CanvasGroup showPhotoPromptGroup;

    private void Awake()
    {
        showPhotoPromptGroup = GetComponent<CanvasGroup>();
        showPhotoPromptGroup.alpha = 0f;

        EventBus.Subscribe<PhotoLoweredAtStartEvent>(OnPhotoLoweredAtStartEvent);
    }

    private void OnPhotoLoweredAtStartEvent(PhotoLoweredAtStartEvent e)
    {
        StartCoroutine(FadeInFadeOut(showPhotoPromptGroup, 0.2f, 1.2f, 3f, 1f));
    }

    IEnumerator FadeInFadeOut(CanvasGroup canvasGroup, float initialDelay, float fadeInDuration, float holdTime, float fadeOutDuration)
    {
        if (canvasGroup)
        {
            yield return new WaitForSeconds(initialDelay);

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / fadeInDuration;
                yield return null;
            }

            yield return new WaitForSeconds(holdTime);

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeOutDuration;
                yield return null;
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class ShowPhotoPromptUI : MonoBehaviour
{
    private CanvasGroup showPhotoPromptGroup;
    private bool shouldShowPhotoPrompt = false;

    private void Awake()
    {
        showPhotoPromptGroup = GetComponent<CanvasGroup>();

        Initialize();

        EventBus.Subscribe<PhotoLoweredAtStartEvent>(OnPhotoLoweredAtStartEvent);
        EventBus.Subscribe<TitleScreenLoadedEvent>(e => Initialize());
    }

    private void Initialize()
    {
        showPhotoPromptGroup.alpha = 0f;
        shouldShowPhotoPrompt = false;
    }

    private void OnPhotoLoweredAtStartEvent(PhotoLoweredAtStartEvent e)
    {
        StartCoroutine(FadeInFadeOut(showPhotoPromptGroup, 0.2f, 1.2f, 3f, 1f));
    }

    IEnumerator FadeInFadeOut(CanvasGroup canvasGroup, float initialDelay, float fadeInDuration, float holdTime, float fadeOutDuration)
    {
        if (canvasGroup)
        {
            shouldShowPhotoPrompt = true;

            yield return new WaitForSeconds(initialDelay);

            while (canvasGroup.alpha < 1 && shouldShowPhotoPrompt)
            {
                canvasGroup.alpha += Time.deltaTime / fadeInDuration;
                yield return null;
            }

            yield return new WaitForSeconds(holdTime);

            while (canvasGroup.alpha > 0 && shouldShowPhotoPrompt)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeOutDuration;
                yield return null;
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class PowerDown : MonoBehaviour
{
    public float textFadeDuration;
    public float textShowDuration;
    public float screenFadeDuration;
    public CanvasGroup textCanvasGrewp;
    public CanvasGroup fadeCanvasGrewp;

    private GlitchEffect glitchEffect;

    //private void Awake()
    //{
    //    glitchEffect = FindObjectOfType<GlitchEffect>();
    //    Init();
    //}

    //private void OnLevelWasLoaded(int level)
    //{
    //    Init();
    //}

    public void Init()
    {
        glitchEffect.enabled = false;
        textCanvasGrewp.alpha = 0;
        fadeCanvasGrewp.alpha = 0;
    }

    [SubscribeGlobal]
    public void HandlePowerDown (PowerDownEvent e)
    {
        glitchEffect = FindObjectOfType<GlitchEffect>();
        glitchEffect.enabled = true;
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            // uncomment to test
            HandlePowerDown(new PowerDownEvent());
        }
    }

    IEnumerator FadeOut()
    {
        while (textCanvasGrewp.alpha < 1)
        {
            textCanvasGrewp.alpha += Time.deltaTime / textFadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(textShowDuration);

        while (fadeCanvasGrewp.alpha < 1)
        {
            fadeCanvasGrewp.alpha += Time.deltaTime / textFadeDuration;
            yield return null;
        }
        EventBus.PublishEvent(new ReturnHomeEvent());
    }
}

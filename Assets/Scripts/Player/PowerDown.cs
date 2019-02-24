using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PowerDown : MonoBehaviour
{
    public float powerDownGlitchIntensitay;
    public float textFadeDuration;
    public float textShowDuration;
    public float screenFadeDuration;
    public CanvasGroup textCanvasGrewp;
    public CanvasGroup fadeCanvasGrewp;

    private GlitchEffect glitchEffect;

    private void Awake()
    {
        Init();
        EventBus.Subscribe<PowerDownEvent>(HandlePowerDown);
    }

    //private void OnLevelWasLoaded(int level)
    //{
    //    Init();
    //}

    public void Init()
    {
        glitchEffect = FindObjectOfType<GlitchEffect>();
        glitchEffect.enabled = false;
        textCanvasGrewp.alpha = 0;
        fadeCanvasGrewp.alpha = 0;
    }

    public void HandlePowerDown (PowerDownEvent e)
    {
        glitchEffect = FindObjectOfType<GlitchEffect>();
        glitchEffect.enabled = true;
        glitchEffect.overallIntensity = powerDownGlitchIntensitay;

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Xtra_SFX/Fail_State");

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Destroy(GameObject.Find("BootupText")); // Why are we doing this? So the glitch effect plays?
        Debug.Log("IT'S Gone");
        textCanvasGrewp.alpha = 0;

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

        // Set scene transition fade up before moving the player in preparation for the handoff to GameStateHome fading it out
        CanvasGroup sceneTransitionFade = GameObject.Find("SceneTransitionFade").GetComponent<CanvasGroup>();
        sceneTransitionFade.alpha = 1f;

        // Before sending player back home, destroy held item
        PickupHolder pickupHolder = FindObjectOfType<PickupHolder>();
        if (pickupHolder)
        {
            Pickupable heldItem = pickupHolder.GetHeldItem();
            if (heldItem)
            {
                Object.Destroy(heldItem.gameObject);
            }
        }

        FindObjectOfType<PlayableDirector>().enabled = false;
        EventBus.PublishEvent(new ReturnHomeEvent());
    }
}

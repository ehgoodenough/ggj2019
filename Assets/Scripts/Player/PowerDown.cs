﻿using System.Collections;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            // uncomment to test
            // HandlePowerDown(new PowerDownEvent());
        }
    }

    IEnumerator FadeOut()
    {
        Destroy(GameObject.Find("BootupText"));
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

        FindObjectOfType<PlayableDirector>().enabled = false;
        EventBus.PublishEvent(new ReturnHomeEvent());
    }
}

using System.Collections;
using UnityEngine;

public class LightningFlash : MonoBehaviour
{
    public float noiseFrequency = 1f;
    public float timeScale = 5f;
    public float noiseWeight = 1f;

    public float lerpAmount = 0;
    public float lerpDuration = .2f;

    public Vector2 strikeDurationRange;
    public Vector2 timeBetweenStrikesRange;

    public CanvasGroup lightningCanvasGroup;

    private void Awake()
    {
        lightningCanvasGroup = GetComponent<CanvasGroup>();
        Invoke("Strike", Random.Range(timeBetweenStrikesRange.x, timeBetweenStrikesRange.y));
    }

    float Noise()
    {
        float noise = 0f;

        for (int i = 0; i < 3; i++)
        {
            float oct = Mathf.PerlinNoise(timeScale * Time.time * Mathf.Pow(noiseFrequency, (float)i), 0f) - .5f;
            noise += oct * Mathf.Pow(.5f, (float)i);
        }


        return noise;
    }

    public void Strike()
    {
        // TODO: Play thunder sound
        StartCoroutine(StartStrike());
    }

    IEnumerator StartStrike()
    {
        while (lerpAmount < 1)
        {
            lerpAmount += Time.deltaTime / lerpDuration;
            yield return null;
        }

        lerpAmount = 1;
        yield return new WaitForSeconds(Random.Range(strikeDurationRange.x, strikeDurationRange.y));
        StartCoroutine(EndStrike());
    }

    IEnumerator EndStrike()
    {
        while (lerpAmount > 0)
        {
            lerpAmount -= Time.deltaTime / lerpDuration;
            yield return null;
        }

        lerpAmount = 0;

        // Schedule next lightning strike
        Invoke("Strike", Random.Range(timeBetweenStrikesRange.x, timeBetweenStrikesRange.y));
    }

    void Update()
    {
        lightningCanvasGroup.alpha = Noise() * noiseWeight * lerpAmount;
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerMovementPowerDownHandler : MonoBehaviour, ISlowingModifier
{
    public float slowingDuration = 1.5f;

    private float powerDownModifier = 1f;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        EventBus.Subscribe<PowerDownEvent>(OnPowerDownEvent);
        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
    }

    public float GetSlowingModifier()
    {
        return powerDownModifier;
    }

    private IEnumerator LerpPowerDownModifier(float startModifierValue, float endModifierValue, float lerpDuration)
    {
        while (Mathf.Min(startModifierValue, endModifierValue) < powerDownModifier)
        {
            powerDownModifier += Mathf.Sign(endModifierValue - startModifierValue) * Time.deltaTime / lerpDuration;
            yield return null;
        }
    }

    private void OnPowerDownEvent(PowerDownEvent e)
    {
        StartCoroutine(LerpPowerDownModifier(powerDownModifier, 0f, slowingDuration));
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovementPowerDownHandler.OnEnterHomeEvent()");
        powerDownModifier = 1f;
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string footstepEvent;
    [FMODUnity.EventRef]
    public string indoorFootstepEvent;
    public float strideLength = 5.0f;

    private Vector3 lastFootstepLocation;
    private bool muteFootsteps = true;
    private bool outside = false;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        Initialize();

        playerMovement = GetComponent<PlayerMovement>();

        EventBus.Subscribe<EnterTitleScreenEvent>(e => Initialize());
        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
    }

    private void Initialize()
    {
        muteFootsteps = true;
        outside = false;
    }

    void FixedUpdate()
    {
        if (!muteFootsteps && playerMovement.GetCurrentSpeed() > 0 && Vector3.Distance(lastFootstepLocation, transform.position) > strideLength)
        {
            PlayFootstep();
        }
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        muteFootsteps = false;
        outside = false;
    }

    private void OnExitHomeEvent(ExitHomeEvent e)
    {
        muteFootsteps = true;
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterCityEvent()");
        muteFootsteps = false;
        outside = true;
    }

    private void OnExitCityEvent(ExitCityEvent e)
    {
        muteFootsteps = true;
    }

    private void PlayFootstep()
    {
        lastFootstepLocation = transform.position;
        if (outside)
        {
            FMODUnity.RuntimeManager.PlayOneShot(footstepEvent, transform.position);
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(indoorFootstepEvent, transform.position);
        }
    }
}

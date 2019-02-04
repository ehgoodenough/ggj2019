using UnityEngine;
using System.Collections;

public class RobotDogAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string footstepEvent;
    [FMODUnity.EventRef]
    public string indoorFootstepEvent;
    public float strideLength = 2.0f;

    [SerializeField]
    private Vector3 lastFootstepLocation;
    private bool muteFootsteps = false;
    private bool outside = false;

    private RobotDogAI dogAI;

    private void Awake()
    {
        dogAI = GetComponent<RobotDogAI>();

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
    }

    private void Update()
    {
        if (!muteFootsteps && dogAI.GetCurrentSpeed() > 0 && Vector3.Distance(lastFootstepLocation, transform.position) > strideLength)
        {
            PlayFootstep();
        }
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

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("RobotDogAudio.OnEnterHomeEvent()");
        muteFootsteps = false;
        outside = false;
    }

    private void OnExitHomeEvent(ExitHomeEvent e)
    {
        muteFootsteps = true;
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("RobotDogAudio.OnEnterCityEvent()");
        muteFootsteps = false;
        outside = true;
    }

    private void OnExitCityEvent(ExitCityEvent e)
    {
        muteFootsteps = true;
    }
}

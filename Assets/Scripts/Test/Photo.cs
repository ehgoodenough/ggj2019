using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

public class Photo : MonoBehaviour
{
    public const float HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS = 4.2f;

    private float rotationSpeed = 180.0f;

    private float time;

    private bool showPhoto = false;

    private bool playerCanRaisePhoto = false;

    private FMOD.Studio.EventInstance introVoiceOverInstance;

    // Only use this method for player input
    public void ShowPhoto()
    {
        if (playerCanRaisePhoto)
        {
            showPhoto = true;
        }
    }

    // Only use this method for player input
    public void HidePhoto()
    {
        if (playerCanRaisePhoto)
        {
            showPhoto = false;
        }
    }

    private void Awake()
    {
        time = HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS; // Ensure that photo is down to start

        EventBus.Subscribe<ExitTitleScreenEvent>(OnExitTitleScreenEvent);
        EventBus.Subscribe<EnterTitleScreenEvent>(OnEnterTitleScreen);
    }

    void Update()
    {
        time += Time.deltaTime;
        // const float HOW_LONG_TO_KEEP_IT_UP_AT_START = 5f;
        if (time < HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS || showPhoto) { // Input.GetKey("space")) {
            if (transform.localRotation.x > 0) {
                transform.Rotate(new Vector3(-1,0,0) * Time.deltaTime * rotationSpeed);
            }
        } else {
            if(transform.localRotation.x < 0.85) {
                transform.Rotate(new Vector3(1,0,0) * Time.deltaTime * rotationSpeed * 2.0f);
            }
        }
    }

    private void OnEnterTitleScreen(EnterTitleScreenEvent e)
    {
        StopCoroutine(HoldUpPhotoAtStart());
        introVoiceOverInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        introVoiceOverInstance.release();
        playerCanRaisePhoto = false;
    }

    private void OnExitTitleScreenEvent(ExitTitleScreenEvent e)
    {
        StartCoroutine(HoldUpPhotoAtStart());
    }

    IEnumerator HoldUpPhotoAtStart()
    {
        // Boot up sequence fades out
        // Glitch sequence ends
        // What is home?
        // Is this HOME?
        // Is THIS home?
        // WHAT is home?

        // Wait for boot up sequence to end
        yield return new WaitForSeconds(5.8f);
        introVoiceOverInstance = FMODUnity.RuntimeManager.CreateInstance("event:/VO/What_Is_Home_Intro");
        introVoiceOverInstance.start();
        introVoiceOverInstance.release();
        // While glitching, ask "What is home?"

        // Wait until "Is this HOME?" to pull up photo
        yield return new WaitForSeconds(2.7f);
        time = 0f; // while time is less than duration to keep at start, photo is raised

        // Lower Photo on "Is THIS home?"
        yield return new WaitForSeconds(HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS);

        // When photo is fully lowered, release movement on "WHAT is home?"
        yield return new WaitForSeconds(3.4f);
        Debug.Log("Photo Lowered At Start");
        EventBus.PublishEvent(new PhotoLoweredAtStartEvent());
        playerCanRaisePhoto = true;

        yield return new WaitForSeconds(10.0f);
        EventBus.PublishEvent(new OpeningVoiceLineDoneEvent());
    }

    public void HoldUpPhotoForCinematic()
    {
        time = 0;
    }
}

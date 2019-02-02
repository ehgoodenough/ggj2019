using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

public class Photo : MonoBehaviour
{
    private const float HOW_LONG_TO_KEEP_IT_UP_AT_START = 4.2f;

    private float rotationSpeed = 180.0f;

    private float time;

    private bool showPhoto = false;

    private bool playerCanRaisePhoto = false;

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
        time = HOW_LONG_TO_KEEP_IT_UP_AT_START; // Ensure that photo is down to start
        EventBus.Subscribe<ExitTitleScreenEvent>(OnExitTitleScreenEvent);
    }

    void Update()
    {
        time += Time.deltaTime;
        // const float HOW_LONG_TO_KEEP_IT_UP_AT_START = 5f;
        if (time < HOW_LONG_TO_KEEP_IT_UP_AT_START || showPhoto) { // Input.GetKey("space")) {
            if (transform.localRotation.x > 0) {
                transform.Rotate(new Vector3(-1,0,0) * Time.deltaTime * rotationSpeed);
            }
        } else {
            if(transform.localRotation.x < 0.85) {
                transform.Rotate(new Vector3(1,0,0) * Time.deltaTime * rotationSpeed * 2.0f);
            }
        }
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
        var intro = FMODUnity.RuntimeManager.CreateInstance("event:/VO/What_Is_Home_Intro");
        intro.start();
        intro.release();
        // While glitching, ask "What is home?"

        // Wait until "Is this HOME?" to pull up photo
        yield return new WaitForSeconds(2.7f);
        time = 0f; // while time is less than duration to keep at start, photo is raised

        // Lower Photo on "Is THIS home?"
        yield return new WaitForSeconds(HOW_LONG_TO_KEEP_IT_UP_AT_START);

        // When photo is fully lowered, release movement on "WHAT is home?"
        yield return new WaitForSeconds(3.4f);
        Debug.Log("Photo Lowered At Start");
        EventBus.PublishEvent(new PhotoLoweredAtStartEvent());
        playerCanRaisePhoto = true;
    }
}

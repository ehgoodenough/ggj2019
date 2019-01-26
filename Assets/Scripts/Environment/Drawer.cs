using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : Interactable
{
    public Vector3 openExtendVector;
    public float openCloseDuration;

    AudioSource audioSource;
    public AudioClip openAudio;
    public AudioClip closeAudio;

    public bool isOpen;
    Vector3 closedPosition;
    Vector3 openPosition;
    Vector3 velocity;

    private void Awake()
    {
        closedPosition = transform.position;
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        // put this in update so we can tweak it in inspector
        openPosition = closedPosition + openExtendVector;

        gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, isOpen ? openPosition : closedPosition, ref velocity, openCloseDuration);
    }

    public override void Interact(Pickupable heldItem)
    {
        base.Interact(heldItem);
        isOpen = !isOpen;

        if (isOpen)
        {
            audioSource.Stop();
            audioSource.clip = openAudio;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = closeAudio;
            audioSource.Play();
        }

        StopInteracting(); // this interaction is instantaneous
    }
}

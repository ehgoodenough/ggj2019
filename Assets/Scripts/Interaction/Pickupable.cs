using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : Interactable
{
    private Rigidbody rb;
    private bool isHeld;

    public AudioClip pickingUpClip;
    protected AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return !heldItem;
    }

    public void Pickup(Vector3 holdPoint, Transform parent)
    {
        Debug.Log("Pickupable.Pickup()");
        isHeld = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.transform.parent = parent;
        rb.transform.position = holdPoint;
        rb.transform.rotation = Quaternion.identity;
        rb.detectCollisions = false;
    }

    public void Drop()
    {
        isHeld = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.transform.parent = null;
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }

    public bool IsHeld()
    {
        return isHeld;
    }

    public void PlayPickingUpClip()
    {
        if (pickingUpClip) PlayClip(pickingUpClip);
    }

    public bool IsPlayingClip()
    {
        if (audioSource && audioSource.clip)
        {
            return audioSource.isPlaying;
        }
        return false;
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

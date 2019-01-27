using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorToCity : Interactable
{
    [FMODUnity.EventRef]
    public string doorOpenEvent;
    [FMODUnity.EventRef]
    public string doorCloseEvent;

    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(doorCloseEvent, transform.position);
    }

    // Do not let the players take anything
    // from the house to the city outside.
    public override bool CanInteractWith(Pickupable heldItem)
    {
        return heldItem == null; // Maybe we can bring other objects in and out?
    }

    public override void Interact(Pickupable heldItem)
    {
        EventBus.PublishEvent(new LeaveHomeEvent(heldItem));
        FMODUnity.RuntimeManager.PlayOneShotAttached(doorOpenEvent, FindObjectOfType<PlayerMovement>().gameObject);
    }
}

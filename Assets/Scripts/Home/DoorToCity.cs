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

    private static bool enteredFromTitleScreen = true;

    void Start()
    {
        if (!enteredFromTitleScreen)
        {
            FMODUnity.RuntimeManager.PlayOneShot(doorCloseEvent, transform.position);
        } else
        {
            enteredFromTitleScreen = false; // all other transitions will be between city and home
        }
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

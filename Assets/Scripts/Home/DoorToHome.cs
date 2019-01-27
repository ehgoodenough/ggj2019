using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorToHome : Interactable
{
    // public Transform Item1;
    // public Transform Item2;
    // public Transform Item3;

    [FMODUnity.EventRef]
    public string doorOpenEvent;
    [FMODUnity.EventRef]
    public string doorCloseEvent;

    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(doorCloseEvent, transform.position);
    }

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return true;
    }

    public override void Interact(Pickupable heldItem)
    {
        if(heldItem is ObjectivePickupable) {
            ObjectivePickupable objectiveItem = (ObjectivePickupable)heldItem;
            GameProgress.CompleteObjective(objectiveItem.type);
            Object.Destroy(heldItem.gameObject);
        }

        EventBus.PublishEvent(new ReturnHomeEvent(heldItem));
        FMODUnity.RuntimeManager.PlayOneShotAttached(doorOpenEvent, FindObjectOfType<PlayerMovement>().gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorToHome : Interactable
{
    public override void Interact(Pickupable heldItem)
    {
        // TODO: Use heldItem to add item to the house!
        EventBus.PublishEvent(new ReturnHomeEvent(heldItem));
    }
}

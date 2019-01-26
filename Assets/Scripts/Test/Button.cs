using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactable
{
    public override bool CanInteractWith(Pickupable heldItem)
    {
        return heldItem == null;
    }

    public override void Interact(Pickupable heldItem)
    {
        Debug.Log("Button Pressed");
        EventBus.PublishEvent(new TestOpenGateEvent(this));
    }
}

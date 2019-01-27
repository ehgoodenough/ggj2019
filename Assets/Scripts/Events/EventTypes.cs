using UnityEngine;
using System.Collections.Generic;

public struct DropItemTriggerEvent {
    public Pickupable itemToDrop;
    public DropItemTriggerEvent(Pickupable item) { this.itemToDrop = item; }
}

public struct TestOpenGateEvent
{
    public Button buttonPressed;
    public TestOpenGateEvent(Button buttonPressed) { this.buttonPressed = buttonPressed; }
}

public struct ReturnHomeEvent
{
    public Pickupable heldItem;
    public ReturnHomeEvent(Pickupable heldItem) { this.heldItem = heldItem; }
}

public struct EnterHomeEvent { }

public struct LeaveHomeEvent
{
    public Pickupable heldItem;
    public LeaveHomeEvent(Pickupable heldItem) { this.heldItem = heldItem; }
}

public struct EnterCityEvent { }
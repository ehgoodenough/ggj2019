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
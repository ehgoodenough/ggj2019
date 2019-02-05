using UnityEngine;
using System.Collections.Generic;

public struct TitleScreenStartEvent
{
    public GameObject player;
    public TitleScreenStartEvent(GameObject player) { this.player = player; }
}

public struct DropItemTriggerEvent {
    public Pickupable itemToDrop;
    public DropItemTriggerEvent(Pickupable item) { this.itemToDrop = item; }
}

public struct TestOpenGateEvent
{
    public Button buttonPressed;
    public TestOpenGateEvent(Button buttonPressed) { this.buttonPressed = buttonPressed; }
}

public struct EnterHomeEvent
{
    public GameStateHome homeState;
    public EnterHomeEvent(GameStateHome homeState) { this.homeState = homeState; }
}

public struct EnterCityEvent
{
    public GameStateCity cityState;
    public EnterCityEvent(GameStateCity cityState) { this.cityState = cityState; }
}

public struct ExitTitleScreenEvent { }

public struct ExitHomeEvent { }

public struct ExitCityEvent { }

public struct LeaveHomeEvent
{
    public Pickupable heldItem;
    public LeaveHomeEvent(Pickupable heldItem) { this.heldItem = heldItem; }
}

public struct ReturnHomeEvent
{
    public Pickupable heldItem;
    public ReturnHomeEvent(Pickupable heldItem) { this.heldItem = heldItem; }
}

public struct PowerDownEvent { }

public struct StartGameEvent { }

public struct PlayerStartPositionEvent
{
    public Transform startTransform;
    public PlayerStartPositionEvent(Transform startTransform) { this.startTransform = startTransform; }
}

public struct PhotoLoweredAtStartEvent { }

public struct PauseMenuEngagedEvent { }

public struct PauseMenuDisengagedEvent { }

public struct SwitchFocusToRestartOptionEvent { }

public struct SwitchFocusToResumeOptionEvent { }

public struct SwitchFocusToQuitOptionEvent { }

public struct PlayerHasWonEvent { }

public struct FriendFullyAssembledEvent { }

public struct OpeningVoiceLineDoneEvent { }

public struct ObjectiveItemPickedUpEvent
{
    public ObjectivePickupable objectiveItem;
    public ObjectiveItemPickedUpEvent(ObjectivePickupable objectiveItem) { this.objectiveItem = objectiveItem; }
}

public struct UntouchedObjectiveItemDetectedEvent
{
    public ObjectivePickupable objectiveItem;
    public UntouchedObjectiveItemDetectedEvent(ObjectivePickupable objectiveItem) { this.objectiveItem = objectiveItem; }
}
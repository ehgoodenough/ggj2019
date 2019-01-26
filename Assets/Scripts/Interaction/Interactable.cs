using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool isInteracting = false; // Used for interactions that have a duration
    private bool restrictsMovement = false; // Restricts movement while isInteracting when set to true
    private bool restrictsView = false; // Restricts looking around while isInteracting when set to true
    private Renderer rend;

    public virtual bool CanInteractWith(Pickupable heldItem)
    {
        // By default, the object should not be interactable while the player is already holding something
        // Some objects may be capable of being interacted only when the correct item is being held
        return heldItem == null;
    }

    public virtual void Interact(Pickupable heldItem)
    {
        // Debug.Log("Interactable.Interact()");
        // Debug.Log("interacting with " + gameObject.name + " holding pickupable " + heldItem.name);

        // By default, interactions will be immediate
        // If an interaction will have a duration, set isInteracting to true
        // isInteracting = true;

        // If an interaction has a duration, and something needs to happen when completed, use an event
        // Events can be used for example to trigger when to drop an item during or after an interaction
        // The following can be called at the end of a Coroutine for an interaction with duration
        // EventBus.PublishEvent(new DropItemTriggerEvent(heldItem));
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    public bool IsInteractionRestrictingMovement()
    {
        return restrictsMovement && isInteracting;
    }

    public bool IsInteractionRestrictingView()
    {
        return restrictsView && isInteracting;
    }

    public virtual void StopInteracting()
    {
        isInteracting = false;
    }

    public bool IsVisible()
    {
        if (!rend)
        {
            rend = GetComponentInChildren<Renderer>();
        }

        return rend.isVisible;
    }
}

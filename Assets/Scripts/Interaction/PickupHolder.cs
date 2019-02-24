using UnityEngine;

public class PickupHolder : MonoBehaviour
{
    public Transform holdPoint;

    private InteractionDetector interactionDetector;
    private Pickupable heldItem;

    private void Start()
    {
        interactionDetector = GetComponent<InteractionDetector>();
        EventBus.Subscribe<DropItemTriggerEvent>(OnDropItemTriggerEvent);
    }

    public void TryPickupOrDrop()
    {
        // Debug.Log("TryPickupOrDrop");
        // Debug.Log("heldItem");
        if (heldItem != null)
        {
            DropItem();
        }
        else
        {
            PickupItem();
        }
    }

    public Pickupable GetHeldItem()
    {
        return heldItem;
    }

    void PickupItem()
    {
        if (interactionDetector.GeInteractableInFocus())
        {
            Pickupable pickup = interactionDetector.GeInteractableInFocus().GetComponent<Pickupable>();
            if (pickup && !pickup.IsHeld())
            {
                DoPickup(pickup);
            }
        }
    }

    void DoPickup(Pickupable pickupable)
    {
        pickupable.Pickup(holdPoint.position, transform);
        heldItem = pickupable;
    }

    void DropItem()
    {
        // Debug.Log("Drop Item");
        heldItem.Drop();
        heldItem = null;
    }

    void OnDropItemTriggerEvent(DropItemTriggerEvent e)
    {
        if (e.itemToDrop == heldItem)
        {
            DropItem();
        }
    }
}

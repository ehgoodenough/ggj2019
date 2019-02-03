using UnityEngine;

public class ObjectivePickupable : Pickupable
{
    public enum Type
    {
        Chair,
        Flowers,
        Art
    }

    public Type type;

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return !GameProgress.isObjectiveComplete(type);
    }

    public override void Pickup(Vector3 holdPoint, Transform parent)
    {
        base.Pickup(holdPoint, parent);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactables/Grab_Item", transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/VO/Object_Found", transform.position);
    }
}

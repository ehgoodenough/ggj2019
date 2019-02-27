using UnityEngine;
using UnityEngine.AI;

public class ObjectivePickupable : Pickupable
{
    public enum Type
    {
        Chair,
        Flowers,
        Art
    }

    public Type type;

    private NavMeshObstacle obstacle;

    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();
    }

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return !GameProgress.isObjectiveComplete(type);
    }

    public override void Pickup(Vector3 holdPoint, Transform parent)
    {
        base.Pickup(holdPoint, parent);

        // SetToLayer("HeldObjective"); // Uncomment to test rendering held objective over other layers

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactables/Grab_Item", transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/VO/Object_Found", transform.position);

        GameProgress.RecordObjectiveItemHavingBeenPickedUp(this.type);
        EventBus.PublishEvent(new ObjectiveItemPickedUpEvent(this));

        obstacle.enabled = false;
    }

    public override void Drop()
    {
        base.Drop();

        // SetToLayer("Color"); // Uncomment to test rendering held objective over other layers

        obstacle.enabled = true;
    }

    private void SetToLayer(string layerName)
    {
        Renderer[] rends = this.gameObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in rends)
        {
            rend.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }
}

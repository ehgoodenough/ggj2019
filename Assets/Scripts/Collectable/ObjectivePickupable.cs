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
}

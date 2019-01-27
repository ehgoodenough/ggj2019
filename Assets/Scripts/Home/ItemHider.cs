using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component simply activates or deactivates a component
// in the home or city scene depending on whether the item
// has or hasn't been collected yet.

public class ItemHider : MonoBehaviour
{
    public enum Location
    {
        Home,
        City
    }

    public Location location;
    public ObjectivePickupable.Type type;

    void Awake()
    {
        if(this.location == Location.Home) {
            this.gameObject.SetActive(GameProgress.isObjectiveComplete(this.type));
        } else {
            this.gameObject.SetActive(!GameProgress.isObjectiveComplete(this.type));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectiveItem : MonoBehaviour
{
    public ObjectivePickupable.Type type;

    void Awake()
    {
        this.gameObject.SetActive(GameProgress.isObjectiveComplete(this.type));
    }
}

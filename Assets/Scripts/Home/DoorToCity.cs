using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorToCity : Interactable
{
    // Do not let the players take anything
    // from the house to the city outside.
    public override bool CanInteractWith(Pickupable heldItem)
    {
        return heldItem == null;
    }

    public override void Interact(Pickupable heldItem)
    {
        SceneManager.LoadScene("CityScene");
    }
}

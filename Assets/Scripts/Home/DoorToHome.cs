using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorToHome : Interactable
{
    public CanvasGroup sceneTransitionFade;
    public float fadeDuration = 1f;
    // public Transform Item1;
    // public Transform Item2;
    // public Transform Item3;

    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactables/Door_Close", transform.position);
    }

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return true;
    }

    public override void Interact(Pickupable heldItem)
    {
        // Debug.Log("Interact with DoorToHome whild holding: " + heldItem);
        if (heldItem && heldItem is ObjectivePickupable) {
            // Debug.Log("Held Item is ObjectivePickupable");
            ObjectivePickupable objectiveItem = (ObjectivePickupable)heldItem;
            GameProgress.CompleteObjective(objectiveItem.type);
            Object.Destroy(heldItem.gameObject);
        }

        StartCoroutine(EnterHome(heldItem));
        //EventBus.PublishEvent(new ReturnHomeEvent(heldItem));
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Interactables/Door_Open", FindObjectOfType<PlayerMovement>().gameObject);
    }

    IEnumerator EnterHome(Pickupable heldItem)
    {
        sceneTransitionFade = GameObject.Find("SceneTransitionFade").GetComponent<CanvasGroup>();

        Debug.Log("Entering home");
        sceneTransitionFade.alpha = 0;
        while (sceneTransitionFade.alpha < 1)
        {
            sceneTransitionFade.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        EventBus.PublishEvent(new ReturnHomeEvent(heldItem));
    }
}

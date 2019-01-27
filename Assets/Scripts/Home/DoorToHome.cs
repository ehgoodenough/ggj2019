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

    [FMODUnity.EventRef]
    public string doorOpenEvent;
    [FMODUnity.EventRef]
    public string doorCloseEvent;

    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(doorCloseEvent, transform.position);
    }

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return true;
    }

    public override void Interact(Pickupable heldItem)
    {
        if(heldItem is ObjectivePickupable) {
            ObjectivePickupable objectiveItem = (ObjectivePickupable)heldItem;
            GameProgress.CompleteObjective(objectiveItem.type);
            Object.Destroy(heldItem.gameObject);
        }

        StartCoroutine(EnterHome(heldItem));
        //EventBus.PublishEvent(new ReturnHomeEvent(heldItem));
        FMODUnity.RuntimeManager.PlayOneShotAttached(doorOpenEvent, FindObjectOfType<PlayerMovement>().gameObject);
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

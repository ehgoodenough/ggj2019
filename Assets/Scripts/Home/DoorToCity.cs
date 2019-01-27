using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DoorToCity : Interactable
{
    public CanvasGroup sceneTransitionFade;
    public float fadeDuration = 1f;
    [FMODUnity.EventRef]
    public string doorOpenEvent;
    [FMODUnity.EventRef]
    public string doorCloseEvent;

    private static bool enteredFromTitleScreen = true;

    void Start()
    {
        if (!enteredFromTitleScreen)
        {
            FMODUnity.RuntimeManager.PlayOneShot(doorCloseEvent, transform.position);
        } else
        {
            enteredFromTitleScreen = false; // all other transitions will be between city and home
        }
    }

    // Do not let the players take anything
    // from the house to the city outside.
    public override bool CanInteractWith(Pickupable heldItem)
    {
        return heldItem == null; // Maybe we can bring other objects in and out?
    }

    public override void Interact(Pickupable heldItem)
    {
        //EventBus.PublishEvent(new LeaveHomeEvent(heldItem));
        StartCoroutine(EnterCity(heldItem));
        FMODUnity.RuntimeManager.PlayOneShotAttached(doorOpenEvent, FindObjectOfType<PlayerMovement>().gameObject);
    }

    IEnumerator EnterCity(Pickupable heldItem)
    {
        sceneTransitionFade = GameObject.Find("SceneTransitionFade").GetComponent<CanvasGroup>();

        Debug.Log("Entering city");
        sceneTransitionFade.alpha = 0;
        while (sceneTransitionFade.alpha < 1)
        {
            sceneTransitionFade.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        EventBus.PublishEvent(new LeaveHomeEvent(heldItem));
    }
}

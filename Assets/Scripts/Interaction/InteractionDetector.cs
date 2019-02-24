using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PickupHolder))]
public class InteractionDetector : MonoBehaviour
{
    private PickupHolder pickupHolder;

    private List<Interactable> interactablesInRange = new List<Interactable>();
    private Interactable interactableInFocus;
    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = GetComponentInParent<Camera>();
        // Debug.Log("playerCamera: " + playerCamera);
        pickupHolder = GetComponent<PickupHolder>();

        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
    }

    private void Update()
    {
        UpdateInteractableInFocus();
    }

    private void UpdateInteractableInFocus()
    {
        /* For the moment, we will allow other interactables to come into focus while other interactions are ongoing
        // Do not change the interactableInFocus if it is currently being interacted with
        if (interactableInFocus != null && interactableInFocus.IsInteracting())
        {
            return; // Exit update and do not reset interactableInFocus
        }
        */

        // Check whether a new interactable should now be in focus
        /// Note: A variety of events will affect what is currently the nearest valid interactable
        ///         These events include:   Player moving and/or looking around
        ///                                 Player dropping current item or picking up a new one
        ///                                 New interactable entering or exiting detector
        ///                                 Interactable internal state changing, resulting in CanInteractWith() => true
        ///         This effectively means that we may as well check every frame which item should be in focus
        if (interactablesInRange.Count > 0)
        {
            SortInteractablesInRange();
            foreach (Interactable interactable in interactablesInRange)
            {
                if (!interactable) continue;

                Pickupable pickupable = interactable.GetComponent<Pickupable>();

                // Ignore the currently held item
                if (pickupable && pickupable == pickupHolder.GetHeldItem())
                {
                    continue;
                }

                // Ignore pickupables that are currently enclosed within a container
                if (pickupable && Container.CheckContained(pickupable.gameObject))
                {
                    continue;
                }

                // Ignore interactables if they cannot be interacted with given the currently held item
                if (!interactable.CanInteractWith(pickupHolder.GetHeldItem()))
                {
                    continue;
                }

                // Update outline highlight if nearest valid interactable is different from the previous interactable in focus
                if (interactable != interactableInFocus)
                {
                    if (interactableInFocus != null)
                    {
                        OutlineManager.Instance.UnapplyOutline(interactableInFocus.gameObject); // Highlighted is the Highlander...
                    }
                    OutlineManager.Instance.ApplyOutline(interactable.gameObject); // ...There can only be one!
                    interactableInFocus = interactable;
                }

                return; // Exit once interactableInFocus is found, whether current or new
            }
        }
        // If there are no valid interactables in range, set interactableInFocus to null
        // UnapplyOutline here rather than in OnTriggerExit?
        interactableInFocus = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter: " + other);
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            // Debug.Log("OnTriggerEnter: " + other);
            interactablesInRange.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable)
        {
            interactablesInRange.Remove(interactable);
            OutlineManager.Instance.UnapplyOutline(interactable.gameObject);
        }
    }

    public void PerformInteraction()
    {
        if (interactableInFocus != null)
        {
            if (interactableInFocus.CanInteractWith(pickupHolder.GetHeldItem()))
            {
                interactableInFocus.Interact(pickupHolder.GetHeldItem());
                // Turn off highlighting here when interaction starts?
                // Could there be some object we want to keep highlighted while interacting? e.g. a lever or door?
                // Possibly it would be better to turn off the interaction to display that interaction has commenced
            }
        }
    }

    public void SortInteractablesInRange()
    {
        Utils.SortByAngleFromPlayer(interactablesInRange, playerCamera);
    }

    public Interactable GeInteractableInFocus()
    {
        return interactableInFocus;
    }

    // Is this currntly being used anywhere?
    public void TurnOffHighlighting()
    {
        if (interactableInFocus)
        {
            OutlineManager.Instance.UnapplyOutline(interactableInFocus.gameObject);
        }
    }
    
    private void OnExitCityEvent(ExitCityEvent e)
    {
        // Debug.Log("InteractionDetector.OnExitCityEvent");
        interactablesInRange.Clear();
    }

    private void OnExitHomeEvent(ExitHomeEvent e)
    {
        // Debug.Log("InteractionDetector.OnLeaveHomeEvent");
        interactablesInRange.Clear();
    }
}

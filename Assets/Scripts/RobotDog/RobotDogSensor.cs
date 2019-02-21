using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RobotDogAI))]
public class RobotDogSensor : MonoBehaviour
{
    public float checkFrequency = 1f;
    public float sensorRadius = 10f;
    public bool isCityDorg = false;
    public LayerMask detectionLayers;

    private RobotDogAI dorgAI;
    private ObjectivePickupable lastDetectedObjectiveItem;

    private void Awake()
    {
        dorgAI = GetComponent<RobotDogAI>();

        GameProgress.LogObjectivesPickedUp();
        // EventBus.Subscribe<ObjectiveItemPickedUpEvent>(OnObjectiveItemHasBeenPickedUpEvent);
    }

    private void Start()
    {
        StartCoroutine(CheckForNearbyObjectiveItems());
    }

    IEnumerator CheckForNearbyObjectiveItems()
    {
        // Debug.Log("Start Coroutine CheckForNearbyObjectiveItems()");
        while (isCityDorg)
        {
            // Note: Importantly, we are depending on objective items being on the "Color" layer
            //          If they are ever to change layers, this will need to be edited
            Collider[] colliders = Physics.OverlapSphere(dorgAI.player.transform.position, sensorRadius, detectionLayers);

            // Debug.Log("# Colliders: " + colliders.Length);
            if (colliders.Length > 0)
            {
                foreach (Collider collider in colliders)
                {
                    ObjectivePickupable objectiveItem = collider.GetComponent<ObjectivePickupable>();
                    // Debug.Log("objectiveItem: " + objectiveItem);
                    if (objectiveItem && objectiveItem != lastDetectedObjectiveItem && !GameProgress.HasObjectiveItemBeenPickedUp(objectiveItem.type))
                    {
                        // TODO: Have Dorg investigate and call attention to this object
                        Debug.Log("Objective not yet picked up DETECTED");
                        lastDetectedObjectiveItem = objectiveItem;
                        EventBus.PublishEvent(new UntouchedObjectiveItemDetectedEvent(objectiveItem));
                    }
                }
            }
            else
            {
                lastDetectedObjectiveItem = null;
            }

            yield return new WaitForSeconds(checkFrequency);
        }
    }

    /*
    private void OnObjectiveItemHasBeenPickedUpEvent(ObjectiveItemPickedUpEvent e)
    {
        Debug.Log("OnObjectiveItemHasBeenPickedUpEvent ( " + e.objectiveItem.type.ToString() + " )");
    }
    */

    private void OnDrawGizmosSelected()
    {
        if (dorgAI && dorgAI.player)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            Gizmos.DrawWireSphere(dorgAI.player.transform.position, sensorRadius);
        }
    }
}

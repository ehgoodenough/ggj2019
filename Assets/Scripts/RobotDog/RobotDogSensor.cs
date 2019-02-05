using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotDogSensor : MonoBehaviour
{
    public bool checkForObjectives = true;
    public float checkFrequency = 1f;
    public float sensorRadius = 10f;

    private void Awake()
    {
        GameProgress.LogObjectivesPickedUp();
        EventBus.Subscribe<ObjectiveItemPickedUpEvent>(OnObjectiveItemHasBeenPickedUpEvent);
    }

    private void Start()
    {
        StartCoroutine(CheckForNearbyObjectiveItems());
    }

    IEnumerator CheckForNearbyObjectiveItems()
    {
        while (true)
        {
            // TODO: Only check for objectives while out in the city
            if (checkForObjectives)
            {
                // Note: Importantly, we are depending on objective items being on the "ColorAndOutline" layer
                //          If they are ever to change layers, this will need to be edited
                // TODO: Shall we make the sensor originate from the player position instead?
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, sensorRadius, LayerMask.NameToLayer("ColorAndOutline"));

                foreach (Collider collider in colliders)
                {
                    ObjectivePickupable objectiveItem = collider.GetComponent<ObjectivePickupable>();
                    if (objectiveItem && !GameProgress.HasObjectiveItemBeenPickedUp(objectiveItem.type))
                    {
                        // TODO: Have Dorg investigate and call attention to this object
                        Debug.Log("Objective not yet picked up DETECTED");
                    }
                }
            }

            yield return new WaitForSeconds(checkFrequency);
        }
    }

    private void OnObjectiveItemHasBeenPickedUpEvent(ObjectiveItemPickedUpEvent e)
    {
        Debug.Log("OnObjectiveItemHasBeenPickedUpEvent ( " + e.objectiveItem.type.ToString() + " )");
    }

    // TODO: Add On Gizmos indicator showing Sensor range
}

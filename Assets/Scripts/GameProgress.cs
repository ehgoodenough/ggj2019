using System;
using UnityEngine;
using System.Collections.Generic;

public static class GameProgress
{
    public static float homeSaturationLevel = 0;
    
    // Track collected IDs
    public static HashSet<Vector3> collectedSet = new HashSet<Vector3>();

    public static int NumCollected
    {
        get
        {
            return collectedSet.Count;
        }
    }

    public static bool IsIdCollected(Vector3 position)
    {
        return collectedSet.Contains(position);
    }

    public static void Collect(Vector3 position)
    {
        collectedSet.Add(position);
    }

    // Track objective types completed
    public static bool hasJustCompletedObjective = false;
    public static HashSet<ObjectivePickupable.Type> completedObjectives = new HashSet<ObjectivePickupable.Type>();

    public static int NumObjectivesComplete
    {
        get
        {
            return completedObjectives.Count;
        }
    }

    public static bool isObjectiveComplete(ObjectivePickupable.Type type)
    {
        return completedObjectives.Contains(type);
    }

    public static void CompleteObjective(ObjectivePickupable.Type type)
    {
        hasJustCompletedObjective = true;
        completedObjectives.Add(type);
    }

    // Track objective items picked up (at least once)
    private static HashSet<ObjectivePickupable.Type> pickedUpObjectives = new HashSet<ObjectivePickupable.Type>();

    public static int NumPickedUp
    {
        get
        {
            return pickedUpObjectives.Count;
        }
    }

    public static bool HasObjectiveItemBeenPickedUp(ObjectivePickupable.Type type)
    {
        return pickedUpObjectives.Contains(type);
    }

    public static void RecordObjectiveItemHavingBeenPickedUp(ObjectivePickupable.Type type)
    {
        pickedUpObjectives.Add(type);
        LogObjectivesPickedUp();
    }

    public static void LogObjectivesPickedUp()
    {
        foreach (ObjectivePickupable.Type type in Enum.GetValues(typeof(ObjectivePickupable.Type)))
        {
            Debug.Log(type.ToString() + " Objective Item Picked Up: " + GameProgress.HasObjectiveItemBeenPickedUp(type));
        }
    }
}

using System;
using UnityEngine;
using System.Collections.Generic;

public static class GameProgress
{
    public static float homeSaturationLevel = 0;
    
    // Track collected IDs
    public static HashSet<string> collectedIds = new HashSet<string>();

    public static int NumCollected
    {
        get
        {
            return collectedIds.Count;
        }
    }

    public static bool IsIdCollected(string id)
    {
        return collectedIds.Contains(id);
    }

    public static void Collect(string id)
    {
        collectedIds.Add(id);
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

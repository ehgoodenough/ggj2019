using System.Collections.Generic;

public static class GameProgress
{
    public static HashSet<string> collectedIds = new HashSet<string>();
    public static int NumCollected
    {
        get
        {
            return collectedIds.Count;
        }
    }
    public static HashSet<ObjectivePickupable.Type> completedObjectives = new HashSet<ObjectivePickupable.Type>();

    public static bool IsIdCollected(string id)
    {
        return collectedIds.Contains(id);
    }

    public static void Collect(string id)
    {
        collectedIds.Add(id);
    }

    public static bool isObjectiveComplete(ObjectivePickupable.Type type)
    {
        return completedObjectives.Contains(type);
    }

    public static void CompleteObjective(ObjectivePickupable.Type type)
    {
        completedObjectives.Add(type);
    }
}

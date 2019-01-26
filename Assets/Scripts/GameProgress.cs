using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static bool IsIdCollected(string id)
    {
        return collectedIds.Contains(id);
    }

    public static void Collect(string id)
    {
        collectedIds.Add(id);
    }
}

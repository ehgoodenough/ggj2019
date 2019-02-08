using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// NOTE: Extension methods are organized in alphabetical order by the class that they extend

public static class Extensions
{
    // EXTENSIONS TO: HashSet<T>

    /// <summary>
    /// Adds a collection of items to the HashSet and returns the new items that have been added to the HashSet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hashSet"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
    {
        HashSet<T> itemsAdded = new HashSet<T>();
        foreach (var item in collection)
        {
            if (hashSet.Add(item))
            {
                itemsAdded.Add(item);
            }
        }
        return itemsAdded;
    }

    // EXTENSIONS TO: IList<T>

    /// <summary>
    /// Returns a reversed copy of the list (rather than reversing the list in place)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Reversed<T>(this IList<T> list)
    {
        List<T> newList = new List<T>(list);
        newList.Reverse();
        return newList;
    }

    /// <summary>
    /// Shuffles the list in place
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Produces a copy of the list with the items in a randomized order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> ShuffledList<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        List<T> newList = new List<T>(list);
        int n = newList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = newList[k];
            newList[k] = newList[n];
            newList[n] = value;
        }
        return newList;
    }

    // EXTENSIONS TO: Queue<T>

    /// <summary>
    /// Allows adding of a collection to a queue (enqueues in order of the items in collection)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queue"></param>
    /// <param name="collection"></param>
    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            queue.Enqueue(item);
        }
    }

    // EXTENSIONS TO: Stack<T>

    /// <summary>
    /// Allows adding of a collection on top of a stack (adds in order of items in collection)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="collection"></param>
    public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            stack.Push(item);
        }
    }

    // EXTENSIONS TO Vector2

    /// <summary>
    /// Returns a Vector3 with X from the Vector2 X coordinate, Y as 0 (zero), and Z from the Vector2 Y coordinate
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector3 XZ(this Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }

    // EXTENSIONS TO: Vector3

    /// <summary>
    /// Returns a Vector2 with X from the Vector3 X coordinate and Y from the Vector3 Z coordinate
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}

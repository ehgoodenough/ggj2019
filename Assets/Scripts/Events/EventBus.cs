using UnityEngine;
using System.Collections.Generic;
using System;

public static class EventBus
{
    private static Dictionary<Type, Dictionary<GameObject, List<Delegate>>> events
        = new Dictionary<Type, Dictionary<GameObject, List<Delegate>>>();
    private static List<GameObject> cleanupList = new List<GameObject>();
    private static int eventsBeingProcessed = 0;

    public static void PublishEvent(object e, bool includeInactive = false)
    {
        cleanupList.Clear();
        Type t = e.GetType();

        if (!events.ContainsKey(t))
        {
            return;
        }

        Dictionary<GameObject, List<Delegate>> eventDict = events[t];

        ++eventsBeingProcessed;
        foreach (List<Delegate> funcs in eventDict.Values)
        {
            cleanupList.Clear();

            // right now global events only work for monobehaviours
            MonoBehaviour target = funcs[0].Target as MonoBehaviour;
            if (target == null)
            {
                // TODO: This needs to get added to a cleanup list
                continue;
            }

            GameObject obj = target.gameObject;
            if (obj == null)
            {
                cleanupList.Add(obj);
            }
            else if (includeInactive || obj.activeInHierarchy)
            {
                foreach (Delegate func in funcs)
                {
                    try
                    {
                        func.DynamicInvoke(e);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Exception during event publish: " + exception);
                    }
                }
            }
        }
        --eventsBeingProcessed;

        foreach (GameObject obj in cleanupList)
        {
            eventDict.Remove(obj);
        }
    }

    public static void Subscribe<T>(Action<T> func) where T : struct
    {
        Subscribe(typeof(T), func);
    }

    public static void Subscribe(Type type, Delegate func)
    {
        if (!events.ContainsKey(type))
        {
            events[type] = new Dictionary<GameObject, List<Delegate>>();
        }

        GameObject target = (func.Target as MonoBehaviour).gameObject;
        if (!events[type].ContainsKey(target))
        {
            events[type][target] = new List<Delegate>();
        }

        events[type][target].Add(func);
    }

    public static void Subscribe<T>(this GameObject obj, Action<T> func)
    {
        EventSubscriber eventSubscriber = obj.GetComponent<EventSubscriber>();
        if (eventSubscriber == null)
        {
            eventSubscriber = obj.AddComponent<EventSubscriber>();
        }

        eventSubscriber.Subscribe<T>(func);
    }

    public static void Unsubscribe<T>(Action<T> func)
    {
        Type type = typeof(T);
        if (events.ContainsKey(typeof(T)))
        {
            GameObject obj = (func.Target as MonoBehaviour).gameObject;
            if (events[type].ContainsKey(obj))
            {
                if (eventsBeingProcessed > 0)
                {
                    cleanupList.Add(obj);
                }
                else
                {
                    events[type][obj].Remove(func);
                }
            }
        }
    }

    public static void Unsubscribe<T>(this GameObject obj, Action<T> func)
    {
        obj.GetComponent<EventSubscriber>().Unsubscribe<T>(func);
    }

    public static void PublishEvent(this GameObject go, object e, bool ignoreIfInactive = true)
    {
        if (!ignoreIfInactive || go.activeInHierarchy)
        {
            EventSubscriber subscriber = go.GetComponent<EventSubscriber>();

            if (subscriber == null)
            {
                subscriber = go.AddComponent<EventSubscriber>();
            }

            subscriber.PublishEvent(e);
        }
    }
}

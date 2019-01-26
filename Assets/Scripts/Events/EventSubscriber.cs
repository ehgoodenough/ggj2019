using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EventSubscriber : MonoBehaviour
{
    private struct EventPair
    {
        public Type type;
        public Delegate del;
    }

    private Dictionary<Type, List<Delegate>> events = new Dictionary<Type, List<Delegate>>();
    private List<EventPair> cleanupList = new List<EventPair>();
    private List<EventPair> addList = new List<EventPair>();

    private int eventsInProgress = 0;

    public void PublishEvent(object obj, bool includeInactive = false)
    {
        if (eventsInProgress == 0)
        {
            ProcessAddsAndRemoves();
        }

        Type type = obj.GetType();
        if (!events.ContainsKey(type))
        {
            return;
        }

        List<Delegate> funcs = events[type];

        ++eventsInProgress;

        try
        {
            foreach (Delegate func in funcs)
            {
                if (func.Target == null)
                {
                    cleanupList.Add(new EventPair { type = type, del = func });
                    return;
                }

                if (!(func.Target is MonoBehaviour))
                {
                    func.DynamicInvoke(obj);
                }
                else
                {
                    MonoBehaviour mb = func.Target as MonoBehaviour;
                    if (mb == null || mb.gameObject == null)
                    {
                        cleanupList.Add(new EventPair { type = type, del = func });
                    }
                    else if (includeInactive || (mb.gameObject.activeInHierarchy && mb.enabled))
                    {
                        func.DynamicInvoke(obj);
                    }
                }
            }
        }
        finally
        {
            --eventsInProgress;
        }
    }

    public void Awake()
    {
        MonoBehaviour[] monoBehaviours = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour)
            {
                RegisterCallbacks(monoBehaviour);
            }
        }
    }

    void ProcessAddsAndRemoves()
    {
        foreach (EventPair pair in cleanupList)
        {
            if (events.ContainsKey(pair.type))
            {
                events[pair.type].Remove(pair.del);
            }
        }
        cleanupList.Clear();

        foreach (EventPair pair in addList)
        {
            if (!events.ContainsKey(pair.type))
            {
                events[pair.type] = new List<Delegate>();
            }
            events[pair.type].Add(pair.del);
        }
        addList.Clear();
    }

    public void Subscribe<T>(Delegate del)
    {
        AddFunction(typeof(T), del);
    }

    public void Unsubscribe<T>(Delegate del)
    {
        // Defer cleanup so we don't interrupt events in progress
        Type type = typeof(T);
        cleanupList.Add(new EventPair { type = type, del = del });
    }

    public void RegisterCallbacks(MonoBehaviour monoBehaviour)
    {
        MethodInfo[] methods = monoBehaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(method);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is Subscribe)
                {
                    Type paramType = method.GetParameters()[0].ParameterType;
                    Delegate del = Delegate.CreateDelegate(Expression.GetActionType(paramType),
                        monoBehaviour, method.Name);
                    AddFunction(paramType, del);
                }
                else if (attribute is SubscribeGlobal)
                {
                    Type paramType = method.GetParameters()[0].ParameterType;
                    Delegate del = Delegate.CreateDelegate(Expression.GetActionType(paramType),
                        monoBehaviour, method.Name);
                    EventBus.Subscribe(paramType, del);
                }
            }
        }
    }

    private void AddFunction(Type type, Delegate del)
    {
        // Defer addition so we don't interrupt events in progress
        addList.Add(new EventPair { type = type, del = del });
    }
}

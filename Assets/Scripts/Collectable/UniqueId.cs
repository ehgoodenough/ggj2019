using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UniqueId : MonoBehaviour
{
    private static HashSet<string> idRegistry = new HashSet<string>();

    public string id;

    void Start()
    {
        while (string.IsNullOrEmpty(id) || idRegistry.Contains(id))
        {
            id = Guid.NewGuid().ToString();
        }
        idRegistry.Add(id);
    }

    void OnDestroy()
    {
        idRegistry.Remove(id);
    }
}

using System;
using UnityEngine;

[ExecuteInEditMode]
public class UniqueId : MonoBehaviour
{
    public string id;

    void Start()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
        }
    }
}

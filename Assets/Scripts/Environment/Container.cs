using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    // Static fields and functions
    static List<Container> containers;
    static List<Container> GetContainers()
    {
        if (containers == null)
        {
            containers = new List<Container>(FindObjectsOfType<Container>());
        }
        return containers;
    }

    public static bool CheckContained(GameObject go)
    {
        foreach (Container c in GetContainers())
        {
            if (c.Contains(go))
            {
                return true;
            }
        }
        return false;
    }

    // Instance fields and functions
    public Door door;
    public Drawer drawer;
    BoxCollider bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        containers = new List<Container>(FindObjectsOfType<Container>());
    }

    public bool Contains(GameObject obj)
    {
        bool thingIsOpen = (door && door.isOpen) || (drawer && drawer.isOpen);
        return !thingIsOpen && bc.bounds.Contains(obj.transform.position);
    }
}

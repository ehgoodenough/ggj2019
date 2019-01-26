using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance;

    private Dictionary<GameObject, Dictionary<Renderer, int>> cachedLayers;

    private static List<string> IgnoredRenderers = new List<string>()
    { };

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        cachedLayers = new Dictionary<GameObject, Dictionary<Renderer, int>>();
        Shader.SetGlobalFloat("isMac", Application.platform == RuntimePlatform.OSXPlayer ? 1 : 0);
    }

    public void ApplyOutline(GameObject objectToOutline)
    {
        if (!cachedLayers.ContainsKey(objectToOutline))
        {
            cachedLayers[objectToOutline] = new Dictionary<Renderer, int>();
        }
        else
        {
            Debug.LogWarning(objectToOutline + " is already outlined.");
        }

        Dictionary<Renderer, int> cachedLayersForObject = cachedLayers[objectToOutline];

        Renderer[] rends = objectToOutline.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in rends)
        {
            if (ShouldApplyOutline(rend))
            {
                //if (rend.GetComponent<Collider>() != null)
                //{
                //    Debug.LogError("Your renderer and layer are on the same collider for object " + rend.gameObject
                //        + " under top-level parent object " + objectToOutline + ". Outline failed.");
                //    return;
                //}

                cachedLayersForObject[rend] = rend.gameObject.layer;
                rend.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
    }

    bool ShouldApplyOutline(Renderer rend)
    {
        System.Type t = rend.GetType();

        foreach (string ignoredName in IgnoredRenderers)
        {
            if (rend.name == ignoredName)
            {
                return false;
            }
        }
        return (t == typeof(MeshRenderer) || t == typeof(SkinnedMeshRenderer));
    }

    public bool HasOutlineApplied(GameObject obj)
    {
        return cachedLayers.ContainsKey(obj);
    }

    public void UnapplyOutline(GameObject obj)
    {
        if (!HasOutlineApplied(obj))
        {
            //Debug.LogError("You can't un-outline something that you didn't ever outline in the first place...");
            return;
        }

        Dictionary<Renderer, int> cachedLayersForObject = cachedLayers[obj];

        Renderer[] rends = obj.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in rends)
        {
            if (cachedLayersForObject.ContainsKey(rend))
            {
                rend.gameObject.layer = cachedLayersForObject[rend];
            }
        }

        cachedLayers.Remove(obj);
    }
}
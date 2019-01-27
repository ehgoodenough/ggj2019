using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static void SortByAngleFromPlayer<T>(List<T> list, Camera cam) where T : MonoBehaviour
    {
        // Debug.Log("Cam: " + cam);
        list.Sort((a, b) =>
        {
            float angleBetweenForwardA = Vector3.Angle(cam.transform.forward, a.transform.position - cam.transform.position);
            float angleBetweenForwardB = Vector3.Angle(cam.transform.forward, b.transform.position - cam.transform.position);
            return angleBetweenForwardA.CompareTo(angleBetweenForwardB);
        });
    }

    public static void OnNextFrame(MonoBehaviour mb, System.Action action)
    {
        mb.StartCoroutine(OnNextFrameCoroutine(action));
    }

    private static IEnumerator OnNextFrameCoroutine(System.Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }

}

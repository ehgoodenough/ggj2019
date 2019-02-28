using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldObjectiveCamera : MonoBehaviour
{
    public Camera cam;

    private void Awake()
    {
        // For now, let's keep this disabled
        // EventBus.Subscribe<EnterHomeEvent>(e => cam.enabled = true);
        // EventBus.Subscribe<EnterTitleScreenEvent>(e => cam.enabled = false);
    }
}

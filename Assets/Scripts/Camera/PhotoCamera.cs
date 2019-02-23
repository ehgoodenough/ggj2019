using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCamera : MonoBehaviour
{
    public Camera cam;

    private void Awake()
    {
        EventBus.Subscribe<EnterHomeEvent>(e => cam.enabled = true);
        EventBus.Subscribe<EnterTitleScreenEvent>(e => cam.enabled = false);
    }
}

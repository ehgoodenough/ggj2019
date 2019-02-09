using UnityEngine;
using System.Collections;

public class PlayerSpawning : MonoBehaviour
{
    private Transform startTransformForCurrentScene;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);

        EventBus.Subscribe<PlayerStartPositionEvent>(OnPlayerStartPositionEvent);
    }

    private void Update()
    {
        // In case the player falls off the map, let's just put them back at the start
        if (startTransformForCurrentScene != null && this.transform.position.y < -10f)
        {
            PlacePlayerAtTransform(startTransformForCurrentScene);
        }
    }

    private void PlacePlayerAtTransform(Transform startTransform)
    {
        // Debug.Log("PlacePlayerAtTransform");
        if (startTransform)
        {
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;
        }
    }
    
    private void OnPlayerStartPositionEvent(PlayerStartPositionEvent e)
    {
        // Debug.Log("OnPlayerStartPositionEvent");
        startTransformForCurrentScene = e.startTransform;
        PlacePlayerAtTransform(e.startTransform);
    }
}

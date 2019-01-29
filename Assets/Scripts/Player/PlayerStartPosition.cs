using UnityEngine;
using System.Collections;

public class PlayerStartPosition : MonoBehaviour
{
    private void Awake()
    {
        EventBus.PublishEvent(new PlayerStartPositionEvent(this.transform));
    }
}

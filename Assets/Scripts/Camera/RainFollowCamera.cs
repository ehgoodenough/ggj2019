using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFollowCamera : MonoBehaviour
{
    public Transform track;
    
    void Update()
    {
        transform.position = track.position;
    }
}

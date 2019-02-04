using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    public bool oscillateOn = false;

    public Vector3 amplitude = Vector3.zero;
    public Vector3 rate = Vector3.one;

    public bool randPosOffset = false;
    public Vector3 posOffset = Vector3.zero;

    public bool randTimeOffset = false;
    public Vector3 timeOffset = Vector3.zero;

    private Vector3 defaultPos = Vector3.zero;

    private void Awake()
    {
        Initialize();
    }
    
    /*
    private void OnValidate()
    {
        Initialize(); // This ensures that values are re-initialized when object edited in inspector
    }
    */

    public void Initialize()
    {
        defaultPos = transform.localPosition;

        if (randTimeOffset)
        {
            timeOffset = Random.insideUnitSphere * Mathf.PI;
        }

        if (randPosOffset)
        {
            posOffset = Random.insideUnitSphere * Mathf.PI;
        }
    }

    void Update()
    {
        if (oscillateOn)
        {
            Vector3 pos = transform.localPosition;

            float xPos = (defaultPos.x + posOffset.x) + amplitude.x * Mathf.Sin((Time.time + timeOffset.x) * rate.x);
            float yPos = (defaultPos.y + posOffset.y) + amplitude.y * Mathf.Sin((Time.time + timeOffset.y) * rate.y);
            float zPos = (defaultPos.z + posOffset.z) + amplitude.z * Mathf.Sin((Time.time + timeOffset.z) * rate.z);

            transform.localPosition = new Vector3(xPos, yPos, zPos);
        }
    }
}

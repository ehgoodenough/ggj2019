using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Photo : MonoBehaviour
{
    private float rotationSpeed = 180.0f;

    void Update()
    {
        // Transform child = transform.GetChild(0);
        if(Input.GetKey("space")) {
            // if(child.localPosition.y < -0.5) {
            //     child.Translate(new Vector3(0,1,0) * Time.deltaTime);
            // }
            if(transform.localRotation.x > 0) {
                transform.Rotate(new Vector3(-1,0,0) * Time.deltaTime * rotationSpeed);
            }
        } else {
            // if(child.localPosition.y > -1) {
            //     child.Translate(new Vector3(0,-1,0) * Time.deltaTime);
            // }
            if(transform.localRotation.x < 0.85) {
                transform.Rotate(new Vector3(1,0,0) * Time.deltaTime * rotationSpeed * 2.0f);
            }
        }
        // Debug.Log();
        // update += Time.deltaTime;
        // if (update > 1.0f)
        // {
        //     update = 0.0f;
        //     Debug.Log("Update");
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

public class Photo : MonoBehaviour
{
    private float rotationSpeed = 180.0f;

    private float time = 0;

    private bool showPhoto = false;

    public void ShowPhoto()
    {
        showPhoto = true;
    }

    public void HidePhoto()
    {
        showPhoto = false;
    }

    void Update()
    {
        time += Time.deltaTime;
        const float HOW_LONG_TO_KEEP_IT_UP_AT_START = 10f;
        if (time < HOW_LONG_TO_KEEP_IT_UP_AT_START || showPhoto) { // Input.GetKey("space")) {
            if (transform.localRotation.x > 0) {
                transform.Rotate(new Vector3(-1,0,0) * Time.deltaTime * rotationSpeed);
            }
        } else {
            if(transform.localRotation.x < 0.85) {
                transform.Rotate(new Vector3(1,0,0) * Time.deltaTime * rotationSpeed * 2.0f);
            }
        }
    }
}

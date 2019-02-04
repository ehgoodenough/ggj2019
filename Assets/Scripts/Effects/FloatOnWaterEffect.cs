using UnityEngine;
using System.Collections;

public class FloatOnWaterEffect : MonoBehaviour
{
    public float waterYPosition;
    public Transform floatingTransform;
    public Oscillate oscillate;

    private bool isOnWater;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        // Debug.Log("rb: " + rb);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Debug.Log("Enter collision with Water");

            // Set this object to be level with the water
            Vector3 pos = floatingTransform.position;
            Debug.Log("Other Y Position: " + other.transform.position.y);
            pos.y = waterYPosition; // other.transform.position.y;
            floatingTransform.position = pos;
            floatingTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false;

            // oscillate.Initialize();
            oscillate.oscillateOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Debug.Log("Exit collision with Water");
            // rb.constraints = RigidbodyConstraints.None;
            oscillate.oscillateOn = false;
        }
    }
}

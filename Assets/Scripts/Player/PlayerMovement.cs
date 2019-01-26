using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private Vector3 movementVector;
    private Rigidbody rb;

    void Start()
    {
        movementVector = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * movementVector.z * speed + transform.right * movementVector.x * speed;
        rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }

    public void Move(Vector3 movementVector)
    {
        this.movementVector = movementVector;
    }

    public void AddToYaw(float yaw)
    {
        transform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);
    }
}

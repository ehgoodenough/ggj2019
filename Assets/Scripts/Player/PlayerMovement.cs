using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private Vector3 movementVector;
    private Rigidbody rb;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);
        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
    }

    void Start()
    {
        // Debug.Log("PlayerMovement.Start()");
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

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        // Debug.Log("e: " + e);
        // Debug.Log("e.homeState: " + e.homeState);
        this.transform.position = e.homeState.playerStart.position;
        this.transform.rotation = e.homeState.playerStart.rotation;
        // Debug.Log("position: " + this.transform.position);
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterCityEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        // Debug.Log("e: " + e);
        // Debug.Log("e.cityState: " + e.cityState);
        this.transform.position = e.cityState.playerStart.position;
        this.transform.rotation = e.cityState.playerStart.rotation;
        // Debug.Log("position: " + this.transform.position);

    }
}

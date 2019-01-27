using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;

    [FMODUnity.EventRef]
    public string footstepEvent;
    public float strideLength = 5.0f;

    private float currentSpeed = 0f;
    private Vector3 movementVector;
    private Rigidbody rb;
    [SerializeField]
    private Vector3 lastFootstepLocation;
    private bool muteFootsteps = true;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);
        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
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
        currentSpeed = movementVector.magnitude * speed;
        Vector3 moveDirection = transform.forward * movementVector.z * speed + transform.right * movementVector.x * speed;
        rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);

        if (!muteFootsteps && currentSpeed > 0 && Vector3.Distance(lastFootstepLocation, transform.position) > strideLength)
        {
            PlayFootstep();
        }
    }

    public void Move(Vector3 movementVector)
    {
        this.movementVector = movementVector;
    }

    public void AddToYaw(float yaw)
    {
        transform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        // Debug.Log("e: " + e);
        // Debug.Log("e.homeState: " + e.homeState);
        this.transform.position = e.homeState.playerStart.position;
        this.transform.rotation = e.homeState.playerStart.rotation;
        muteFootsteps = false;
        // Debug.Log("position: " + this.transform.position);
    }

    private void OnExitHomeEvent(ExitHomeEvent e)
    {
        muteFootsteps = true;
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterCityEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        // Debug.Log("e: " + e);
        // Debug.Log("e.cityState: " + e.cityState);
        this.transform.position = e.cityState.playerStart.position;
        this.transform.rotation = e.cityState.playerStart.rotation;
        muteFootsteps = false;
        // Debug.Log("position: " + this.transform.position);

    }

    private void OnExitCityEvent(ExitCityEvent e)
    {
        muteFootsteps = true;
    }

    private void PlayFootstep()
    {
        lastFootstepLocation = transform.position;
        FMODUnity.RuntimeManager.PlayOneShot(footstepEvent, transform.position);
    }
}

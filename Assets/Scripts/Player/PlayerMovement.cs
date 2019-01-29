using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float modifiedSpeed;

    [FMODUnity.EventRef]
    public string footstepEvent;
    [FMODUnity.EventRef]
    public string indoorFootstepEvent;
    public float strideLength = 5.0f;

    private float currentSpeed = 0f;
    private Vector3 movementVector;
    private Rigidbody rb;
    private Vector3 lastFootstepLocation;
    private bool muteFootsteps = true;
    private bool outside = false;

    // private NavMeshAgent agent;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);
        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
        EventBus.Subscribe<PlayerStartPositionEvent>(OnPlayerStartPositionEvent);

        // agent = GetComponent<NavMeshAgent>();
        // agent.speed = speed;
    }

    void Start()
    {
        // Debug.Log("PlayerMovement.Start()");
        movementVector = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        modifiedSpeed = speed * ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 1.6f : 1);
    }

    void FixedUpdate()
    {
        currentSpeed = movementVector.magnitude * modifiedSpeed;
        Vector3 moveDirection = transform.forward * movementVector.z * modifiedSpeed + transform.right * movementVector.x * modifiedSpeed;

        /* Yup, this doesn't work
        if (moveDirection.magnitude > 0f)
        {
            Debug.Log("Move");
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position + moveDirection * Time.deltaTime, out hit, 2f, 0))
            {
                Debug.Log("Found Sample Position");
                agent.Warp(hit.position);
                // agent.SetDestination(hit.position);
            }
        }
        */

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

    private void OnPlayerStartPositionEvent(PlayerStartPositionEvent e)
    {
        if (e.startTransform)
        {
            this.transform.position = e.startTransform.position;
            this.transform.rotation = e.startTransform.rotation;
        }
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        // Debug.Log("e: " + e);
        // Debug.Log("e.homeState: " + e.homeState);
        muteFootsteps = false;
        outside = false;
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
        muteFootsteps = false;
        outside = true;
        // Debug.Log("position: " + this.transform.position);

    }

    private void OnExitCityEvent(ExitCityEvent e)
    {
        muteFootsteps = true;
    }

    private void PlayFootstep()
    {
        lastFootstepLocation = transform.position;
        if (outside)
        {
            FMODUnity.RuntimeManager.PlayOneShot(footstepEvent, transform.position);
        } else
        {
            FMODUnity.RuntimeManager.PlayOneShot(indoorFootstepEvent, transform.position);
        }
    }
}

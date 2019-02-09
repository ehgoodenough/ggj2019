using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Could we make this a more generic ISpeedModifier, which running could also implement?
public interface ISlowingModifier
{
    float GetSlowingModifier();
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Base movement speeds
    public float walkingMaxSpeed;
    public float runningMaxSpeed;

    // Base movement variables
    private bool isRunning = false;
    private float currentMaxSpeed;
    private float currentSpeed = 0f;
    private Vector3 movementVector;
    private bool isMovementRestricted = true;
    private bool isGravityRestricted = true;

    // Private components
    private Rigidbody rb;
    private Transform startTransformForCurrentScene; // Spawning
    private ISlowingModifier[] slowingModifiers;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);
        
        slowingModifiers = GetComponents<ISlowingModifier>();

        EventBus.Subscribe<PlayerStartPositionEvent>(OnPlayerStartPositionEvent);
        EventBus.Subscribe<PhotoLoweredAtStartEvent>(OnPhotoLoweredAtStartEvent);
        EventBus.Subscribe<PlayerHasWonEvent>(OnPlayerHasWonEvent);
    }

    void Start()
    {
        // Debug.Log("PlayerMovement.Start()");
        movementVector = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // For Debug purposes only
        // StartCoroutine(LogCurrentMaxSpeed());
    }

    private void Update()
    {
        currentMaxSpeed = (isRunning ? runningMaxSpeed : walkingMaxSpeed);
        // Perhaps we could turn these into a more generic ISpeedModifier, and running could also be an applied modifier?
        foreach (ISlowingModifier modifier in slowingModifiers)
        {
            currentMaxSpeed *= modifier.GetSlowingModifier();
        }

        // In case the player falls off the map, let's just put them back at the start
        if (startTransformForCurrentScene != null && this.transform.position.y < -10f)
        {
            PlacePlayerAtTransform(startTransformForCurrentScene);
        }
    }

    void FixedUpdate()
    {
        if (!isMovementRestricted)
        {
            currentSpeed = movementVector.magnitude * currentMaxSpeed;
            Vector3 moveDirection = transform.forward * movementVector.z * currentSpeed + transform.right * movementVector.x * currentSpeed;

            rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
        }

        if (!isGravityRestricted)
        {
            rb.AddForce(Physics.gravity, ForceMode.Acceleration);
        }
    }

    public void Move(Vector3 movementVector, bool isRunning = false)
    {
        this.isRunning = isRunning;
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

    public float GetCurrentMaxSpeed()
    {
        return currentMaxSpeed;
    }
    
    public Vector3 GetCurrentMovementVectorInWorldSpace()
    {
        return this.transform.TransformDirection(movementVector.normalized);
    }

    public void RestrictMovement(bool restrictMovement)
    {
        isMovementRestricted = restrictMovement;
    }

    public void RestrictGravity(bool restrictGravity)
    {
        isGravityRestricted = restrictGravity;
    }

    private void OnPlayerHasWonEvent(PlayerHasWonEvent e)
    {
        RestrictMovement(true);
    }

    private void OnPhotoLoweredAtStartEvent(PhotoLoweredAtStartEvent e)
    {
        RestrictMovement(false);
    }

    // Spawning
    private void OnPlayerStartPositionEvent(PlayerStartPositionEvent e)
    {
        // Debug.Log("OnPlayerStartPositionEvent");
        startTransformForCurrentScene = e.startTransform;
        PlacePlayerAtTransform(e.startTransform);
        RestrictGravity(false);
    }

    // Spawning
    private void PlacePlayerAtTransform(Transform startTransform)
    {
        // Debug.Log("PlacePlayerAtTransform");
        if (startTransform)
        {
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;
        }
    }

    /// METHODS FOR DEBUGGING PURPOSES BELOW

    IEnumerator LogCurrentMaxSpeed()
    {
        while (true)
        {
            Debug.Log("Current Max Speed: " + currentMaxSpeed);
            yield return new WaitForSeconds(0.33f);
        }
    }

    private void OnDrawGizmos()
    {
        // Base movement vector
        Gizmos.color = Color.blue;
        Vector3 lineStart = this.transform.position + Vector3.up;
        Vector3 lineEnd = this.transform.position + this.GetCurrentMovementVectorInWorldSpace() * 5f + Vector3.up;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
}

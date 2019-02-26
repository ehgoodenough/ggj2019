using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Could we make this a more generic ISpeedModifier, which running could also implement?
public interface ISlowingModifier
{
    float GetSlowingModifier();
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerGroundChecker))]
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
    private ISlowingModifier[] slowingModifiers;

    /// MONOBEHAVIOR METHODS

    private void Awake()
    {
        slowingModifiers = GetComponents<ISlowingModifier>();

        EventBus.Subscribe<EnterTitleScreenEvent>(e =>
        {
            RestrictGravity(true);
            RestrictMovement(true);
        });

        EventBus.Subscribe<PlayerStartPositionEvent>(e => SpawnAtStartPosition(e.startTransform));
        EventBus.Subscribe<PhotoLoweredAtStartEvent>(e => RestrictMovement(false));
        EventBus.Subscribe<ObjectiveCompletedCutsceneStartEvent>(e => RestrictMovement(true));
        EventBus.Subscribe<ObjectiveCompletedCutsceneEndEvent>(e => RestrictMovement(false));
        EventBus.Subscribe<PlayerHasWonEvent>(e => RestrictMovement(true));
    }

    private void Start()
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
    }

    private void FixedUpdate()
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

    /// MOVEMENT PUBLIC METHODS

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
        if (isMovementRestricted) return Vector3.zero;
        return this.transform.TransformDirection(movementVector.normalized);
    }

    /// MOVEMENT PRIVATE METHODS

    private void RestrictMovement(bool restrictMovement)
    {
        isMovementRestricted = restrictMovement;
    }

    private void RestrictGravity(bool restrictGravity)
    {
        isGravityRestricted = restrictGravity;
    }

    private void SpawnAtStartPosition(Transform startTransform)
    {
        this.transform.position = startTransform.position;
        RestrictGravity(false);
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

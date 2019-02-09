using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Base movement speeds
    public float walkingMaxSpeed;
    public float runningMaxSpeed;

    // Invisible Walls
    public float checkForInvisibleWallsMaxDistance = 5f;
    public LayerMask InvisibleWallLayer;

    // Base movement variables
    private bool isRunning = false;
    private float currentMaxSpeed;
    private float currentSpeed = 0f;
    private Vector3 movementVector;
    private bool isMovementRestricted = true;
    private bool isGravityRestricted = true;

    // Power down speed modifier
    private float powerDownModifier = 1f;

    // Private components
    private Rigidbody rb;
    private Transform startTransformForCurrentScene; // Spawning
    private CapsuleCollider playerCapsuleCollider; // Invisible Walls

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);

        playerCapsuleCollider = GetComponent<CapsuleCollider>();

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<PlayerStartPositionEvent>(OnPlayerStartPositionEvent);
        EventBus.Subscribe<PhotoLoweredAtStartEvent>(OnPhotoLoweredAtStartEvent);
        EventBus.Subscribe<PlayerHasWonEvent>(OnPlayerHasWonEvent);
        EventBus.Subscribe<PowerDownEvent>(OnPowerDownEvent);
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
        currentMaxSpeed = (isRunning ? runningMaxSpeed : walkingMaxSpeed) * powerDownModifier * GetInvisibleWallSlowingModifier();

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

    // Invisible Walls
    private float GetInvisibleWallSlowingModifier()
    {
        if (movementVector.sqrMagnitude == 0)
        {
            // Debug.Log("movementVector.sqrMagnitude == 0");
            return 1f; // Skip raycast if not moving
        }

        Collider[] invisibleWallColliders = Physics.OverlapSphere(this.transform.position, checkForInvisibleWallsMaxDistance, InvisibleWallLayer, QueryTriggerInteraction.Ignore);
        if (invisibleWallColliders.Length == 0)
        {
            // Debug.Log("invisibleWallColliders.Length == 0");
            return 1f;
        }

        // Each Invisible Wall can have a different distance at which they start to slow the player
        // We need to find the Invisible Wall that has the maximum slowing power, i.e. lowest slowing modifier
        float minSlowingModifier = 1f;
        for (int i = 0; i < invisibleWallColliders.Length; i++)
        {
            InvisibleWall invisibleWall = invisibleWallColliders[i].GetComponent<InvisibleWall>();
            if (!invisibleWall || !invisibleWall.slowsToAStop) continue;

            float currentSlowingModifier = invisibleWall.GetNormalizedSlowingModifier(playerCapsuleCollider);
            minSlowingModifier = currentSlowingModifier < minSlowingModifier ? currentSlowingModifier : minSlowingModifier;
        }
        // Debug.Log("minSlowingModifier: " + minSlowingModifier);
        return Mathf.Clamp01(minSlowingModifier + 0.35f); // Allow at least some movement so player doesn't get trapped at (nearly) zero
    }

    IEnumerator LogCurrentMaxSpeed()
    {
        while (true)
        {
            Debug.Log("Current Max Speed: " + currentMaxSpeed);
            yield return new WaitForSeconds(0.33f);
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

    // Powering Down
    private void OnPowerDownEvent(PowerDownEvent e)
    {
        StartCoroutine(LerpPowerDownModifier(powerDownModifier, 0f, 1.5f));
    }

    // Powering Down
    private IEnumerator LerpPowerDownModifier(float startModifierValue, float endModifierValue, float lerpDuration)
    {
        while (Mathf.Min(startModifierValue, endModifierValue) < powerDownModifier)
        {
            powerDownModifier += Mathf.Sign(endModifierValue - startModifierValue) * Time.deltaTime / lerpDuration;
            yield return null;
        }
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

    // Powering Down
    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        powerDownModifier = 1f;
    }

    private void OnDrawGizmos()
    {
        // Invisible Walls
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, checkForInvisibleWallsMaxDistance);

        // Base movement vector
        Gizmos.color = Color.blue;
        Vector3 lineStart = this.transform.position + Vector3.up;
        Vector3 lineEnd = this.transform.position + this.GetCurrentMovementVectorInWorldSpace() * 5f + Vector3.up;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
}

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float walkingMaxSpeed;
    public float runningMaxSpeed;
    public float checkForInvisibleWallsMaxDistance = 5f;
    public LayerMask InvisibleWallLayer;

    [FMODUnity.EventRef]
    public string footstepEvent;
    [FMODUnity.EventRef]
    public string indoorFootstepEvent;
    public float strideLength = 5.0f;

    private bool isRunning = false;
    private float currentMaxSpeed;
    private float currentSpeed = 0f;
    private float powerDownModifier = 1f;
    private Vector3 movementVector;
    private Rigidbody rb;
    private Vector3 lastFootstepLocation;
    private bool muteFootsteps = true;
    private bool outside = false;
    private bool isMovementRestricted = true;
    private bool isGravityRestricted = true;
    private Transform startTransformForCurrentScene;
    private CapsuleCollider playerCapsuleCollider;

    private void Awake()
    {
        // Debug.Log("PlayerMovement.Awake()");
        // Debug.Log("Player Position: " + this.transform.position);

        playerCapsuleCollider = GetComponent<CapsuleCollider>();

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);
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

        if (!muteFootsteps && currentSpeed > 0 && Vector3.Distance(lastFootstepLocation, transform.position) > strideLength)
        {
            PlayFootstep();
        }
    }

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

    private void OnPowerDownEvent(PowerDownEvent e)
    {
        StartCoroutine(LerpPowerDownModifier(powerDownModifier, 0f, 1.5f));
    }

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

    private void OnPlayerStartPositionEvent(PlayerStartPositionEvent e)
    {
        // Debug.Log("OnPlayerStartPositionEvent");
        startTransformForCurrentScene = e.startTransform;
        PlacePlayerAtTransform(e.startTransform);
        RestrictGravity(false);
    }

    private void PlacePlayerAtTransform(Transform startTransform)
    {
        // Debug.Log("PlacePlayerAtTransform");
        if (startTransform)
        {
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;
        }
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerMovement.OnEnterHomeEvent()");
        // Debug.Log("Player Position: " + this.transform.position);
        muteFootsteps = false;
        outside = false;
        powerDownModifier = 1f;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, checkForInvisibleWallsMaxDistance);

        Gizmos.color = Color.blue;
        Vector3 lineStart = this.transform.position + Vector3.up;
        Vector3 lineEnd = this.transform.position + this.GetCurrentMovementVectorInWorldSpace() * 5f + Vector3.up;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
}

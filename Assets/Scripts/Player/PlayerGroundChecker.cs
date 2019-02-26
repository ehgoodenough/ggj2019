using UnityEngine;
using System.Collections;

public class PlayerGroundChecker : MonoBehaviour
{
    public LayerMask groundLayers;
    public float checkFrequency = 0.5f;

    private Collider playerCollider;
    private Rigidbody rb;

    private bool isGrounded = false;
    private bool shouldCheck = false;
    private float distanceToGround;

    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        distanceToGround = playerCollider.bounds.extents.y;

        EventBus.Subscribe<EnterHomeEvent>(e => shouldCheck = true);
        EventBus.Subscribe<ExitHomeEvent>(e => shouldCheck = false);
        EventBus.Subscribe<EnterCityEvent>(e => shouldCheck = true);
        EventBus.Subscribe<ExitCityEvent>(e => shouldCheck = false);
        EventBus.Subscribe<EnterTitleScreenEvent>(e => shouldCheck = false);
    }

    private void Start()
    {
        StartCoroutine(CheckForGround());
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    IEnumerator CheckForGround()
    {
        while (true)
        {
            if (shouldCheck)
            {
                bool wasGrounded = isGrounded;
                isGrounded = Physics.BoxCast(playerCollider.bounds.center, playerCollider.bounds.extents * 0.8f, Vector3.down, Quaternion.identity, distanceToGround + 0.1f, groundLayers);
                
                if (isGrounded && !wasGrounded)
                {
                    EventBus.PublishEvent(new PlayerHitsGroundEvent(rb.velocity));
                }
                else if (!isGrounded && wasGrounded)
                {
                    EventBus.PublishEvent(new PlayerLeavesGroundEvent(rb.velocity));
                }
                
                Debug.Log("Is Grounded: " + isGrounded);
            }
            yield return new WaitForSeconds(checkFrequency);
        }
    }
}

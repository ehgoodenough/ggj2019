using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotDogAI : MonoBehaviour
{
    public GameObject player;
    public float updateDestinationFrequency = 1f;
    public float startFollowingHysteresis = 2f;

    private NavMeshAgent agent;
    private Vector3 nextPosition; // This will be the most recent position of the player
    private Vector3 previousPosition; // This will be the position before the most recent position of the player
    private float startFollowingDistance;

    private PoochAnimator animator;

    private static bool chairHasBeenPickedUp = false;
    private static bool artworkHasBeenPickedUp = false;
    private static bool flowerHasBeenPickedUp = false;

    void Awake()
    {
        Debug.Log("Chair Has Been Picked Up: " + chairHasBeenPickedUp);
        Debug.Log("Artwork Has Been Picked Up: " + artworkHasBeenPickedUp);
        Debug.Log("Flower Has Been Picked Up: " + flowerHasBeenPickedUp);

        animator = GetComponentInChildren<PoochAnimator>();

        // Debug.Log("RobotDogAI.Awake()");
        // Debug.Log("Robot Dog Position: " + this.transform.position);
        agent = GetComponent<NavMeshAgent>();
        startFollowingDistance = agent.stoppingDistance + startFollowingHysteresis;

        PlaceOnNavMesh(this.transform.position);

        // Debug.Log("Player: " + player);
        // Debug.Log("PlayerMovement: " + FindObjectOfType<PlayerMovement>());
        if (player == null) player = FindObjectOfType<PlayerMovement>().gameObject;
        
        EventBus.Subscribe<ObjectiveItemPickedUpEvent>(OnObjectiveItemHasBeenPickedUpEvent);
    }

    void Start()
    {
        // Debug.Log("RobotDogAI.Start()");
        Debug.Assert(player != null, "Robot Dog AI needs to have a reference to the player transform");

        nextPosition = GetNextPositionOnNavMesh(player.transform.position);
        StartCoroutine(FollowPlayer());
    }

    void Update()
    {
        // Change Animation state according to current speed
        float currentSpeed = GetCurrentSpeed();
        float currentSpeedNormalized = currentSpeed / agent.speed;
        // Debug.Log("Current Speed Normalized: " + currentSpeedNormalized);
        if (currentSpeedNormalized > 0.75f )
        {
            animator.Run();
        }
        else if (currentSpeedNormalized > 0.05f)
        {
            animator.Trot();
        }
        else
        {
            animator.Idle();
        }
    }
    
    public float GetCurrentSpeed()
    {
        return agent.velocity.magnitude;
    }

    IEnumerator FollowPlayer()
    {
        while (true)
        {
            previousPosition = nextPosition;
            nextPosition = GetNextPositionOnNavMesh(player.transform.position);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
            {
                agent.SetDestination(nextPosition);
            }
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
        }
    }

    private Vector3 GetNextPositionOnNavMesh(Vector3 samplePosition)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(samplePosition, out hit, 2.5f, 1);
        return positionFound ? hit.position : this.transform.position; // If cannot find position, stay put
    }

    private void PlaceOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(position, out hit, 2.5f, 1);
        this.transform.position = positionFound ? hit.position : this.transform.position; // If cannot find position, stay put
    }

    private void OnObjectiveItemHasBeenPickedUpEvent(ObjectiveItemPickedUpEvent e)
    {
        switch (e.objectiveItem.type)
        {
            case ObjectivePickupable.Type.Art:
                Debug.Log("Artwork Has Been Picked Up");
                if (!artworkHasBeenPickedUp) artworkHasBeenPickedUp = true;
                break;
            case ObjectivePickupable.Type.Chair:
                Debug.Log("Chair Has Been Picked Up");
                if (!chairHasBeenPickedUp) chairHasBeenPickedUp = true;
                break;
            case ObjectivePickupable.Type.Flowers:
                Debug.Log("Flower Has Been Picked Up");
                if (!flowerHasBeenPickedUp) flowerHasBeenPickedUp = true;
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
    }
}

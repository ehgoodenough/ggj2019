using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

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

    private RobotDogAiState currentState;
    private enum RobotDogAiState
    {
        FollowBehindPlayer
    }

    /*
    tree "Root"
	    fallback
		    tree "InvestigateObjectiveItem"
		    FollowBehindPlayer

    tree "InvestigateObjectiveItem"
	    sequence
		    IsValidObjectiveInRange
		    DeterminePointNearObjective
		    GotToPointNearObjective
		    InvestigateObjective
		    CallAttentionToObjective
    */

    void Awake()
    {
        animator = GetComponentInChildren<PoochAnimator>();

        // Debug.Log("RobotDogAI.Awake()");
        agent = GetComponent<NavMeshAgent>();
        startFollowingDistance = agent.stoppingDistance + startFollowingHysteresis;

        PlaceOnNavMesh(this.transform.position);

        // Debug.Log("Player: " + player);
        if (player == null) player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    void Start()
    {
        // Debug.Log("RobotDogAI.Start()");
        Debug.Assert(player != null, "Robot Dog AI needs to have a reference to the player transform");

        // nextPosition = GetNextPositionOnNavMesh(player.transform.position);
        // StartCoroutine(FollowPlayer());
    }

    [Task]
    public void FollowBehindPlayer()
    {
        // Debug.Log("FollowBehindPlayer() [Task]");
        if (Task.current.isStarting)
        {
            currentState = RobotDogAiState.FollowBehindPlayer;
            nextPosition = GetNextPositionOnNavMesh(player.transform.position);
            StartCoroutine(FollowPlayer());
        }

        // Change Animation state according to current speed
        float currentSpeed = GetCurrentSpeed();
        float currentSpeedNormalized = currentSpeed / agent.speed;
        // Debug.Log("Current Speed Normalized: " + currentSpeedNormalized);
        if (currentSpeedNormalized > 0.75f)
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

    IEnumerator FollowPlayer()
    {
        Debug.Log("Start FollowPlayer() Coroutine");
        while (currentState == RobotDogAiState.FollowBehindPlayer)
        {
            previousPosition = nextPosition;
            nextPosition = GetNextPositionOnNavMesh(player.transform.position);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);

            // Check that current state continues to be Follow Behind Player
            if (currentState == RobotDogAiState.FollowBehindPlayer)
            {
                if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
                {
                    agent.SetDestination(nextPosition);
                }
                yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            }
        }
    }

    public float GetCurrentSpeed()
    {
        return agent.velocity.magnitude;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
    }
}

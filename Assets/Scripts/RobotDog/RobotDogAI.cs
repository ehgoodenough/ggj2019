using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotDogAI : MonoBehaviour
{
    public PlayerMovement player;
    public float updateDestinationFrequency = 1f;
    public float startFollowingHysteresis = 2f;
    public float aheadBehindDuration = 6f;

    [Header("Follow Ahead Parameters")]
    public float minimumDistanceToFollowAhead = 5f;
    public float speedDependentDistanceToFollowAhead = 5f;
    public float lookingDirectionWeight = 0.33f;
    public float movingDirectionWeight = 1.67f;

    private NavMeshAgent agent;
    private Vector3 nextPosition; // This will be the most recent position of the player
    private Vector3 previousPosition; // This will be the position before the most recent position of the player
    private float startFollowingDistance;

    private PoochAnimator animator;

    private DorgTask currentTask;
    public enum DorgTask
    {
        FollowPlayerAhead,
        FollowPlayerBehind,
        InvestigateObjectiveItem
    }

    /*
    tree "Root"
	fallback
		while IsValidObjectiveInRange
			tree "InvestigateObjectiveItem"
		while not IsValidObjectiveInRange
            fallback
                while ShouldFollowPlayerAhead
                    FollowPlayerAhead
                while not ShouldFollowPlayerAhead
			        FollowPlayerBehind

    tree "InvestigateObjectiveItem"
	    GoToPointNearObjective
    */

    void Awake()
    {
        animator = GetComponentInChildren<PoochAnimator>();

        // Debug.Log("RobotDogAI.Awake()");
        agent = GetComponent<NavMeshAgent>();
        startFollowingDistance = agent.stoppingDistance + startFollowingHysteresis;
        
        PlaceOnNavMesh(this.transform.position);
        nextPosition = this.transform.position;
        previousPosition = this.transform.position;

        // Debug.Log("Player: " + player);
        if (player == null) player = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        // Debug.Log("RobotDogAI.Start()");
        Debug.Assert(player != null, "Robot Dog AI needs to have a reference to the player transform");

        // nextPosition = GetNextPositionOnNavMesh(player.transform.position);
        // StartCoroutine(FollowPlayer());
    }

    public float GetCurrentSpeed()
    {
        return agent.velocity.magnitude;
    }

    private void PlaceOnNavMesh(Vector3 position)
    {
        this.transform.position = GetPositionOnNavMesh(position);
    }

    public Vector3 GetPositionOnNavMesh(Vector3 samplePosition)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(samplePosition, out hit, 2.5f, 1);
        return positionFound ? hit.position : this.transform.position; // If cannot find position, return current position
    }

    public DorgTask GetCurrentDorgTask()
    {
        return currentTask;
    }

    /// <summary>
    /// This should only be called on Task.current.isStarting of the first task of any Dorg AI tree
    /// </summary>
    public void SetCurrentDorgTask(DorgTask newTask)
    {
        currentTask = newTask;
    }

    /// Determine wheather to follower player from ahead or behind

    [Task]
    public bool ShouldFollowPlayerAhead()
    {
        // For now, let's alternate evenly
        // TODO: modify this to be random (coherent noise function?)
        float stateDuration = aheadBehindDuration; // number of seconds to be following ahead, then number of seconds to be following behind
        bool shouldFollowAhead = Mathf.Sin(Time.time * Mathf.PI / stateDuration) > 0f;
        return shouldFollowAhead;
    }

    /// Handle Follow Player Behind Task

    [Task]
    public void FollowPlayerBehind()
    {
        // Debug.Log("FollowPlayerBehind() [Task]");
        if (Task.current.isStarting)
        {
            Debug.Log("Starting FollowPlayerBehind [Task]");
            currentTask = DorgTask.FollowPlayerBehind;
            nextPosition = GetPositionOnNavMesh(player.transform.position);
            StartCoroutine(FollowBehindPlayer());
        }

        HandleFollowPlayerBehindAnimation();
    }

    IEnumerator FollowBehindPlayer()
    {
        Debug.Log("Start FollowBehindPlayer() Coroutine");
        while (currentTask == DorgTask.FollowPlayerBehind)
        {
            previousPosition = nextPosition;
            nextPosition = GetPositionOnNavMesh(player.transform.position);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);

            // Check that current state continues to be Follow Player Ahead
            if (currentTask == DorgTask.FollowPlayerBehind)
            {
                if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
                {
                    agent.SetDestination(nextPosition);
                }
                yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            }
        }
    }

    private void HandleFollowPlayerBehindAnimation()
    {
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

    /// Handle Follow Player Ahead Task

    [Task]
    public void FollowPlayerAhead()
    {
        // Debug.Log("FollowPlayerBehind() [Task]");
        if (Task.current.isStarting)
        {
            Debug.Log("Starting FollowPlayerAhead [Task]");
            currentTask = DorgTask.FollowPlayerAhead;
            nextPosition = GetPositionOnNavMesh(player.transform.position);
            StartCoroutine(FollowAheadOfPlayer());
        }

        HandleFollowPlayerAheadAnimation();
    }

    IEnumerator FollowAheadOfPlayer()
    {
        Debug.Log("Start FollowAheadOfPlayer() Coroutine");
        while (currentTask == DorgTask.FollowPlayerAhead)
        {
            previousPosition = nextPosition;
            nextPosition = GetPositionOnNavMesh(GetPointAheadOfPlayer(minimumDistanceToFollowAhead, speedDependentDistanceToFollowAhead));
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);

            // Check that current state continues to be Follow Player Ahead
            if (currentTask == DorgTask.FollowPlayerAhead)
            {
                if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
                {
                    agent.SetDestination(nextPosition);
                }
                yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            }
        }
    }

    private Vector3 GetPointAheadOfPlayer(float minimumDistanceAhead, float speedDependentDistanceAhead)
    {
        float currentPlayerSpeedNormalized = player.GetCurrentSpeed() / player.GetCurrentMaxSpeed();
        float distanceAhead = (minimumDistanceAhead + currentPlayerSpeedNormalized * speedDependentDistanceAhead);
        Vector3 directionPlayerIsLooking = player.transform.forward * distanceAhead;
        Vector3 directionPlayerIsGoing = player.GetCurrentMovementVectorInWorldSpace() * distanceAhead;
        return player.transform.position + (directionPlayerIsLooking * lookingDirectionWeight + directionPlayerIsGoing * movingDirectionWeight);
    }

    private void HandleFollowPlayerAheadAnimation()
    {
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

    /// Handle Editor Gizmo Displays

    private void OnDrawGizmosSelected()
    {
        if (currentTask == DorgTask.FollowPlayerBehind || currentTask == DorgTask.FollowPlayerAhead)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
        }
        
        if (currentTask == DorgTask.FollowPlayerAhead)
        {
            Gizmos.color = Color.cyan;
            Vector3 targetPosition = GetPointAheadOfPlayer(minimumDistanceToFollowAhead, speedDependentDistanceToFollowAhead);
            Gizmos.DrawLine(player.transform.position + Vector3.up, targetPosition + Vector3.up);

            Gizmos.color = Color.blue;
            Vector3 lineStart = player.transform.position + Vector3.up;
            Vector3 lineEnd = player.transform.position + player.GetCurrentMovementVectorInWorldSpace() * 5f + Vector3.up;
            Gizmos.DrawLine(lineStart, lineEnd);
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 lineStart = player.transform.position + Vector3.up;
        Vector3 lineEnd = player.transform.position + player.GetCurrentMovementVectorInWorldSpace() * 5f + Vector3.up;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
    */
}

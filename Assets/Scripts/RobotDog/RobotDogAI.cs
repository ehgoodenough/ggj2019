using System;
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

    private RobotDogSensor dorgSensor;
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
        dorgSensor = GetComponent<RobotDogSensor>();
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
        if (!dorgSensor.isCityDorg) return false;

        // For now, let's alternate evenly
        // TODO: modify this to be random (coherent noise function?)
        float stateDuration = aheadBehindDuration; // number of seconds to be following ahead, then number of seconds to be following behind
        float gameProgressModifier = ((float)(GameProgress.NumObjectivesComplete + 2) / (float)(Enum.GetValues(typeof(ObjectivePickupable.Type)).Length + 3));
        // bool shouldFollowAhead = Mathf.Sin(Time.time * Mathf.PI / stateDuration) + (gameProgressModifier - 1) > 0f;

        // float sinValue = Mathf.Sin(Time.time * Mathf.PI / stateDuration) + (gameProgressModifier - 1);
        // float sinValueRandomized = sinValue + 0.5f * (Mathf.PerlinNoise(Time.time / stateDuration * 0.5f, 0f) - 0.5f);

        float stateDurationRandomized = stateDuration * (1 + 0.15f * (Mathf.PerlinNoise(Time.time / stateDuration, 0f) - 0.5f));
        float sinValueRandomized = Mathf.Sin(Time.time * Mathf.PI / stateDurationRandomized) + (gameProgressModifier - 1);

        // Debug.Log("Randomized Sin Value: " + sinValueRandomized);

        bool shouldFollowAhead = sinValueRandomized > 0f;
        return shouldFollowAhead;
    }

    /// Handle Follow Player Behind Task

    [Task]
    public void FollowPlayerBehind()
    {
        // Debug.Log("FollowPlayerBehind() [Task]");
        if (dorgSensor.isCityDorg)
        {
            if (Task.current.isStarting)
            {
                Debug.Log("Starting FollowPlayerBehind [Task]");
                currentTask = DorgTask.FollowPlayerBehind;
                nextPosition = GetPositionOnNavMesh(player.transform.position);
                StartCoroutine(FollowBehindPlayer());
            }

            HandleFollowPlayerBehindAnimation();
        }
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
            // nextPosition = GetPositionOnNavMesh(GetPointAheadOfPlayer(minimumDistanceToFollowAhead, speedDependentDistanceToFollowAhead));
            Vector3 prospectiveTarget = GetPositionOnNavMesh(GetPointAheadOfPlayer(minimumDistanceToFollowAhead, speedDependentDistanceToFollowAhead));
            NavMeshPath prospectivePath = new NavMeshPath();
            if (agent.CalculatePath(prospectiveTarget, prospectivePath))
            {
                if (!ValidatePathToDestination(prospectivePath, 2f))
                {
                    // Pick a point halfway between Dorg and either the first raycast hit or the original prospective target
                    RaycastHit hit;
                    Vector3 center = player.transform.position + Vector3.up;
                    Vector3 halfExtents = Vector3.one * 0.25f;
                    Vector3 direction = prospectiveTarget - player.transform.position;
                    Vector3 endPoint = Physics.BoxCast(center, halfExtents, direction, out hit) ? hit.point : prospectiveTarget;
                    Vector3 bisectedDestination = GetPositionOnNavMesh((endPoint - this.transform.position) * 0.5f);

                    // If this alternative destination is still not valid, we'll just follow the player as usual
                    if (!agent.CalculatePath(bisectedDestination, prospectivePath) || !ValidatePathToDestination(prospectivePath, 2f))
                    {
                        agent.CalculatePath(GetPositionOnNavMesh(player.transform.position), prospectivePath);
                    }
                }
            }
            nextPosition = prospectivePath.corners[prospectivePath.corners.Length - 1];
            if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
            {
                agent.SetPath(prospectivePath);
            }
            yield return new WaitForSeconds(updateDestinationFrequency);
        }
    }

    private Vector3 GetPointAheadOfPlayer(float minimumDistanceAhead, float speedDependentDistanceAhead)
    {
        float currentPlayerSpeedNormalized = player.GetCurrentSpeed() / player.GetCurrentMaxSpeed();
        float distanceAhead = (minimumDistanceAhead + currentPlayerSpeedNormalized * speedDependentDistanceAhead);
        float gameProgressModifier = ((float)(GameProgress.NumObjectivesComplete + 1) / (float)(Enum.GetValues(typeof(ObjectivePickupable.Type)).Length + 1));
        float distanceModified = distanceAhead * (0.25f + 0.75f * gameProgressModifier);
        Vector3 directionPlayerIsLooking = player.transform.forward * distanceModified * lookingDirectionWeight;
        Vector3 directionPlayerIsGoing = player.GetCurrentMovementVectorInWorldSpace() * distanceModified * movingDirectionWeight;
        Vector3 randomOffset = UnityEngine.Random.insideUnitCircle.XZ() * 2f;
        return (player.transform.position + randomOffset) + (directionPlayerIsLooking + directionPlayerIsGoing);
    }

    private bool ValidatePathToDestination(NavMeshPath prospectivePath, float pathToDirectDistanceRatioTolerance)
    {
        if (prospectivePath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            Vector3 startingPoint = prospectivePath.corners[0];
            Vector3 endingPoint = prospectivePath.corners[prospectivePath.corners.Length - 1];
            float directDistance = (endingPoint - startingPoint).magnitude;
            float pathDistance = GetLengthOfNavMeshPath(prospectivePath);
            return pathDistance < directDistance * pathToDirectDistanceRatioTolerance;
        }
    }

    private float GetLengthOfNavMeshPath(NavMeshPath path)
    {
        float sum = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            sum += Vector3.Distance(path.corners[i], path.corners[i - 1]);
        }
        return sum;
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
        
        if (currentTask == DorgTask.FollowPlayerAhead && player != null)
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

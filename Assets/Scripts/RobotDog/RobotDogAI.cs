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

    private DorgTask currentTask;
    public enum DorgTask
    {
        FollowBehindPlayer,
        InvestigateObjectiveItem
    }

    /*
    tree "Root"
	fallback
		while IsValidObjectiveInRange
			tree "InvestigateObjectiveItem"
		while not IsValidObjectiveInRange
			FollowBehindPlayer

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
        if (player == null) player = FindObjectOfType<PlayerMovement>().gameObject;
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

    /// Handle Follow Behind Player Task

    [Task]
    public void FollowBehindPlayer()
    {
        // Debug.Log("FollowBehindPlayer() [Task]");
        if (Task.current.isStarting)
        {
            Debug.Log("Starting FollowBehindPlayer [Task]");
            currentTask = DorgTask.FollowBehindPlayer;
            nextPosition = GetPositionOnNavMesh(player.transform.position);
            StartCoroutine(FollowPlayer());
        }

        HandleFollowBehindPlayerAnimation();
    }

    private void HandleFollowBehindPlayerAnimation()
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

    IEnumerator FollowPlayer()
    {
        Debug.Log("Start FollowPlayer() Coroutine");
        while (currentTask == DorgTask.FollowBehindPlayer)
        {
            previousPosition = nextPosition;
            nextPosition = GetPositionOnNavMesh(player.transform.position);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);

            // Check that current state continues to be Follow Behind Player
            if (currentTask == DorgTask.FollowBehindPlayer)
            {
                if (Vector3.Distance(this.transform.position, nextPosition) > startFollowingDistance)
                {
                    agent.SetDestination(nextPosition);
                }
                yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            }
        }
    }

    /// Handle Editor Gizmo Displays

    private void OnDrawGizmosSelected()
    {
        if (currentTask == DorgTask.FollowBehindPlayer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotDogAI : MonoBehaviour
{
    public Transform player;
    public float updateDestinationFrequency = 1f;

    private NavMeshAgent agent;
    private Vector3 nextPosition; // This will be the most recent position of the player
    private Vector3 previousPosition; // This will be the position before the most recent position of the player

    private void Awake()
    {
        Debug.Assert(player != null, "Robot Dog AI needs to have a reference to the player transform");

        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        nextPosition = GetNextPositionOnNavMesh(player.position);
        StartCoroutine(FollowPlayer());
    }

    IEnumerator FollowPlayer()
    {
        while (true)
        {
            previousPosition = nextPosition;
            nextPosition = GetNextPositionOnNavMesh(player.position);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
            agent.SetDestination(nextPosition);
            yield return new WaitForSeconds(updateDestinationFrequency * 0.5f);
        }
    }

    private Vector3 GetNextPositionOnNavMesh(Vector3 samplePosition)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(samplePosition, out hit, 2.5f, 1);
        return positionFound ? hit.position : this.transform.position; // If cannot find position, stay put
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
    }
}

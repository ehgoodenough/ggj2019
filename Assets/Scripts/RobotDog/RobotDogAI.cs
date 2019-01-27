﻿using System.Collections;
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

    private void Awake()
    {
        // Debug.Log("RobotDogAI.Awake()");
        // Debug.Log("Robot Dog Position: " + this.transform.position);
        agent = GetComponent<NavMeshAgent>();
        startFollowingDistance = agent.stoppingDistance + startFollowingHysteresis;

        PlaceOnNavMesh(this.transform.position);

        // Debug.Log("Player: " + player);
        // Debug.Log("PlayerMovement: " + FindObjectOfType<PlayerMovement>());
        if (player == null) player = FindObjectOfType<PlayerMovement>().gameObject;

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
    }

    private void Start()
    {
        // Debug.Log("RobotDogAI.Start()");
        Debug.Assert(player != null, "Robot Dog AI needs to have a reference to the player transform");

        nextPosition = GetNextPositionOnNavMesh(player.transform.position);
        StartCoroutine(FollowPlayer());
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(previousPosition, Vector3.one * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(nextPosition, Vector3.one * 0.5f);
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("RobotDogAI.OnEnterHomeEvent()");
        // Debug.Log("Robot Dog Position: " + this.transform.position);
        // PlaceOnNavMesh(e.homeState.dogStart.position);
        // this.transform.rotation = e.homeState.dogStart.rotation;
        // Debug.Log("Position: " + this.transform.position);
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("RobotDogAI.OnEnterCityEvent()");
        // Debug.Log("Robot Dog Position: " + this.transform.position);
        // PlaceOnNavMesh(e.cityState.dogStart.position);
        // this.transform.rotation = e.cityState.dogStart.rotation;
    }

    private void PlaceOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(position, out hit, 2.5f, 1);
        this.transform.position = positionFound ? hit.position : this.transform.position; // If cannot find position, stay put
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Panda;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RobotDogAI))]
[RequireComponent(typeof(RobotDogSensor))]
public class RobotDogInvestigateObjective : MonoBehaviour
{
    /*
    tree "InvestigateObjectiveItem"
	    while IsValidObjectiveInRange
            sequence
		        DeterminePointNearObjective
		        GotToPointNearObjective
		        InvestigateObjective
		        CallAttentionToObjective
    */

    public float sensorRangeHysteresis = 5f;
    public float timeInRangeToBeValid = 2f;

    private NavMeshAgent agent;
    private RobotDogAI dorgAI;
    private RobotDogSensor dorgSensor;
    private PoochAnimator animator;
    private ObjectivePickupable untouchedObjectiveItem;
    private float timeElapsedInRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        dorgAI = GetComponent<RobotDogAI>();
        dorgSensor = GetComponent<RobotDogSensor>();
        animator = GetComponentInChildren<PoochAnimator>();
        EventBus.Subscribe<UntouchedObjectiveItemDetectedEvent>(OnUntouchedObjectiveItemDetectedEvent);
    }

    private void Update()
    {
        if (untouchedObjectiveItem)
        {
            // Debug.Log("Untouched Objective Item: " + untouchedObjectiveItem);
            // Debug.Log("Dorg AI: " + dorgAI);
            // Debug.Log("Player: " + dorgAI.player);
            if (Vector3.Distance(dorgAI.player.transform.position, untouchedObjectiveItem.transform.position) <= dorgSensor.sensorRadius + sensorRangeHysteresis)
            {
                // TODO: Do not increment time while game is paused
                timeElapsedInRange += Time.deltaTime;
            }
            else
            {
                Debug.Log("Untouched Objective Item Fell Out Of VALID Range");
                untouchedObjectiveItem = null;
                timeElapsedInRange = 0f;
            }
        }
    }

    private void OnUntouchedObjectiveItemDetectedEvent(UntouchedObjectiveItemDetectedEvent e)
    {
        // What if we have more than one objective item in range...?
        Debug.Log("OnUntouchedObjectiveItemDetectedEvent( " + e.objectiveItem + " )");
        if (!untouchedObjectiveItem) timeElapsedInRange = 0f; // Continuing incrementing current time elapsed if one has already been in range
        untouchedObjectiveItem = e.objectiveItem;
    }

    [Task] 
    public bool IsValidObjectiveInRange()
    {
        if (untouchedObjectiveItem && timeElapsedInRange >= timeInRangeToBeValid)
        {
            // Debug.Log("Untouched Objective Item IN VALID RANGE!");
            dorgAI.SetCurrentDorgTask(RobotDogAI.DorgTask.InvestigateObjectiveItem);
            return true;
        }
        return false;
    }

    [Task]
    public void GoToPointNearObjective()
    {
        if (!untouchedObjectiveItem) Task.current.Fail();

        if (Task.current.isStarting)
        {
            Debug.Log("Start GoToPointNearObjective [Task]");
            // TODO: Do we want to make any adjustments to agent parameters for this BT tree?
            // For now, let's just get whatever the closest point on the NavMesh is
            Vector3 destination = dorgAI.GetPositionOnNavMesh(untouchedObjectiveItem.transform.position);
            agent.SetDestination(destination);
        }

        HandleInvestigateObjectiveAnimation();
    }

    private void HandleInvestigateObjectiveAnimation()
    {
        if (animator)
        {
            // Change Animation state according to current speed
            float currentSpeed = dorgAI.GetCurrentSpeed();
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
                // TODO: Only bark once or periodically, otherwise go Idle
                // TODO: Only Bark when close enough to the objective item
                //          otherwise go Idle (the item may not be close enough to the navMesh)
                animator.Bark();
            }
        }
    }
}

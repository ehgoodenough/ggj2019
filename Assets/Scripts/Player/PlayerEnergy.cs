using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour
{
    private enum EnergyState
    {
        Recharging,
        Depleting,
        Idle
    }

    public float startingEnergy = 10f;
    [SerializeField]
    private float currentEnergy;

    public float initialMaxEnergy = 50f;
    public float incrementMaxAmount = 10f;
    private float currentMaxEnergy;

    public float rechargingRate = 1.25f;
    public float baselineDepletionRate = 0.1f;
    public float walkingDepletionRate = 0.15f;
    public float runningDepletionRate = 0.3f;
    // public float maxDepletionRate = 0.25f;

    private EnergyState currentEnergyState = EnergyState.Idle;
    private EnergyState previousEnergyState = EnergyState.Idle;

    private PlayerMovement movement;
    private Rigidbody rb;

    private float logFrequency = 1f;

    private StateMachine gameStateMachine;

    bool isPoweringDown = false;

    private void Awake()
    {
        currentEnergy = startingEnergy;
        currentMaxEnergy = initialMaxEnergy;

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);

        EventBus.Subscribe<PauseMenuEngagedEvent>(OnPauseMenuEngagedEvent);
        EventBus.Subscribe<PauseMenuDisengagedEvent>(OnPauseMenuDisengagedEvent);

        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        gameStateMachine = FindObjectOfType<GameStateTitleScreen>().GetComponent<StateMachine>();
    }

    private void Start()
    {
        StartCoroutine(LogCurrentEnergy());
    }

    private void Update()
    {
        if (gameStateMachine.currentState.GetType() != typeof(GameStateTitleScreen))
        {
            // if (gameStateMachine.currentState.GetType() == typeof(GameStateHome))
            if (currentEnergyState == EnergyState.Recharging)
            {
                HandleRecharging();
            }
            // else if (gameStateMachine.currentState.GetType() == typeof(GameStateCity))
            else if (currentEnergyState == EnergyState.Depleting)
            {
                HandleDepleting();
            }
        }

        /* Uncomment if you want to cheat
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentEnergy = currentMaxEnergy;
        }
        */
    }

    IEnumerator LogCurrentEnergy()
    {
        while (true)
        {
            // Debug.Log("Current Energy: " + currentEnergy);
            if (currentEnergyState == EnergyState.Depleting)
            {
                // Debug.Log("Current Speed: " + movement.GetCurrentSpeed());
                // Debug.Log("Max Speed: " + movement.speed);
                // Debug.Log("Normalized Speed: " + (movement.GetCurrentSpeed() / movement.speed));
                // Debug.Log("Depletion Amount: " + CalculateDepletionAmount(logFrequency));
            }
            yield return new WaitForSeconds(logFrequency);
        }
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public float GetCurrentMaxEnergy()
    {
        return currentMaxEnergy;
    }

    public void IncrementMaxEnergy()
    {
        currentMaxEnergy += incrementMaxAmount;
    }

    private void HandleRecharging()
    {
        isPoweringDown = false;
        currentEnergy = Mathf.Clamp(currentEnergy + rechargingRate * Time.deltaTime, 0f, currentMaxEnergy);
    }

    private void HandleDepleting()
    {
        if (!isPoweringDown)
        {
            currentEnergy = Mathf.Clamp(currentEnergy - CalculateDepletionAmount(Time.deltaTime), 0f, currentMaxEnergy);
            if (currentEnergy == 0)
            {
                isPoweringDown = true;
                EventBus.PublishEvent(new PowerDownEvent());
                // Debug.Log("Power Down Event");
            }
        }
    }

    private float CalculateDepletionAmount(float depletionTime)
    {
        float currentSpeed = movement.GetCurrentSpeed();
        float walkingNormalized = Mathf.Clamp01(currentSpeed / movement.walkingMaxSpeed);
        float runningNormalized = Mathf.Clamp01((currentSpeed - movement.walkingMaxSpeed) / (movement.runningMaxSpeed - movement.walkingMaxSpeed));
        float currentDepletionRate = baselineDepletionRate + (walkingDepletionRate * walkingNormalized) + (runningNormalized * runningNormalized);
        return currentDepletionRate * depletionTime;

        /*
        float normalizedSpeed = movement.GetCurrentSpeed() / movement.runningMaxSpeed;
        float depletionAmount = (baselineDepletionRate + normalizedSpeed * (maxDepletionRate - baselineDepletionRate)) * depletionTime;
        return depletionAmount;
        */
    }

    private void OnPauseMenuEngagedEvent(PauseMenuEngagedEvent e)
    {
        previousEnergyState = currentEnergyState;
        currentEnergyState = EnergyState.Idle;
    }

    private void OnPauseMenuDisengagedEvent(PauseMenuDisengagedEvent e)
    {
        currentEnergyState = previousEnergyState;
    }

    private void OnEnterHomeEvent(EnterHomeEvent e)
    {
        // Debug.Log("PlayerEnergy.OnEnterHomeEvent()");
        currentEnergyState = EnergyState.Recharging;
    }

    private void OnEnterCityEvent(EnterCityEvent e)
    {
        // Debug.Log("PlayerEnergy.OnEnterCityEvent()");
        currentEnergyState = EnergyState.Depleting;
    }

    private void OnExitHomeEvent(ExitHomeEvent e)
    {
        // Debug.Log("PlayerEnergy.OnExitHomeEvent()");
        currentEnergyState = EnergyState.Idle;
    }

    private void OnExitCityEvent(ExitCityEvent e)
    {
        // Debug.Log("PlayerEnergy.OnExitCityEvent()");
        currentEnergyState = EnergyState.Idle;
    }
}

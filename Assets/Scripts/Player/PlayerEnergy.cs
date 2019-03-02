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

    public float startingEnergy = 5f;
    [SerializeField]
    private float currentEnergy;

    public float initialMaxEnergy = 50f;
    public float incrementMaxAmount = 10f;
    private float currentMaxEnergy;

    public float rechargingRate = 3.0f;
    public float baselineDepletionRate = 0.1f;
    public float walkingDepletionRate = 0.15f;
    public float runningDepletionRate = 0.3f;

    private EnergyState currentEnergyState = EnergyState.Idle;
    private EnergyState previousEnergyState = EnergyState.Idle;

    private PlayerMovement movement;
    private Rigidbody rb;

    private float logFrequency = 1f;

    bool isPoweringDown = false;

    private void Awake()
    {
        Initialize();

        EventBus.Subscribe<IntroBootUpTextCompleteEvent>(e => 
            {
                Initialize();
                currentEnergyState = EnergyState.Recharging;
            });

        EventBus.Subscribe<EnterHomeEvent>(e => currentEnergyState = EnergyState.Recharging);
        EventBus.Subscribe<EnterCityEvent>(e => currentEnergyState = EnergyState.Depleting);
        EventBus.Subscribe<ExitHomeEvent>(e => currentEnergyState = EnergyState.Idle);
        EventBus.Subscribe<ExitCityEvent>(e => currentEnergyState = EnergyState.Idle);

        EventBus.Subscribe<PauseMenuEngagedEvent>(OnPauseMenuEngagedEvent);
        EventBus.Subscribe<PauseMenuDisengagedEvent>(OnPauseMenuDisengagedEvent);

        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Initialize()
    {
        previousEnergyState = EnergyState.Idle;
        currentEnergyState = EnergyState.Idle;

        currentEnergy = startingEnergy;
        currentMaxEnergy = initialMaxEnergy;
    }

    private void Start()
    {
        // StartCoroutine(LogCurrentEnergy());
    }

    private void Update()
    {
        if (currentEnergyState == EnergyState.Recharging)
        {
            HandleRecharging();
        }
        else if (currentEnergyState == EnergyState.Depleting)
        {
            HandleDepleting();
        }

        /* Uncomment if you want to cheat
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentEnergy = currentMaxEnergy;
        }
        */
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
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public float GetCurrentMaxEnergy()
    {
        return currentMaxEnergy;
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

    /// METHODS BELOW FOR DEBUG PURPOSES ONLY

    IEnumerator LogCurrentEnergy()
    {
        while (true)
        {
            // Debug.Log("Current Energy: " + currentEnergy);
            if (currentEnergyState == EnergyState.Depleting)
            {
                Debug.Log("Current Speed: " + movement.GetCurrentSpeed());
                Debug.Log("Max Speed: " + movement.GetCurrentMaxSpeed());
                Debug.Log("Normalized Speed: " + (movement.GetCurrentSpeed() / movement.GetCurrentMaxSpeed()));
                Debug.Log("Depletion Amount: " + CalculateDepletionAmount(logFrequency));
            }
            yield return new WaitForSeconds(logFrequency);
        }
    }
}

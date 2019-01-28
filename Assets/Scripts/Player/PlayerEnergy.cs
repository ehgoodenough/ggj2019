﻿using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour
{
    private enum EnergyState
    {
        Recharging,
        Depleting,
        Idle
    }

    public float startingEnergy = 25f;
    [SerializeField]
    private float currentEnergy;

    public float initialMaxEnergy = 50f;
    public float incrementMaxAmount = 10f;
    private float currentMaxEnergy;

    public float rechargingRate = 1f;
    public float baselineDepletionRate = 0.1f;
    public float maxDepletionRate = 1f;

    private EnergyState currentEnergyState = EnergyState.Idle;

    private PlayerMovement movement;
    private Rigidbody rb;

    private float logFrequency = 1f;

    private StateMachine gameStateMachine;

    private void Awake()
    {
        currentEnergy = startingEnergy;
        currentMaxEnergy = initialMaxEnergy;

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHomeEvent);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCityEvent);
        EventBus.Subscribe<ExitHomeEvent>(OnExitHomeEvent);
        EventBus.Subscribe<ExitCityEvent>(OnExitCityEvent);

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
            if (gameStateMachine.currentState.GetType() == typeof(GameStateHome))
            {
                HandleRecharging();
            }
            else if (gameStateMachine.currentState.GetType() == typeof(GameStateCity))
            {
                HandleDepleting();
            }
        }

        /*
        switch (currentEnergyState)
        {
            case EnergyState.Recharging:
                HandleRecharging();
                break;
            case EnergyState.Depleting:
                HandleDepleting();
                break;
            default:
                break;
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
        currentEnergy = Mathf.Clamp(currentEnergy + rechargingRate * Time.deltaTime, 0f, currentMaxEnergy);
    }

    private void HandleDepleting()
    {
        currentEnergy = Mathf.Clamp(currentEnergy - CalculateDepletionAmount(Time.deltaTime), 0f, currentMaxEnergy);
        if (currentEnergy == 0)
        {
            EventBus.PublishEvent(new PowerDownEvent());
        }
    }

    private float CalculateDepletionAmount(float depletionTime)
    {
        float normalizedSpeed = movement.GetCurrentSpeed() / movement.speed;
        float depletionAmount = (baselineDepletionRate + normalizedSpeed * (maxDepletionRate - baselineDepletionRate)) * depletionTime;
        return depletionAmount;
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

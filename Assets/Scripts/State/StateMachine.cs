using System;
using UnityEngine;
using UnityEngine.Networking;

public class StateMachine : MonoBehaviour
{
    public State currentState;

    private void Awake()
    {
        if (currentState != null)
        {
            currentState.Enter();
        }
        else
        {
            Debug.LogError("Initial state missing: " + this.gameObject.name);
            this.enabled = false;
        }
    }

    void Update()
    {
        currentState.DoUpdate();
    }

    public T GetState<T>() where T : State
    {
        T state = GetComponent<T>();
        if (state == null)
        {
            state = gameObject.AddComponent<T>();
            state.enabled = false;
        }
        return state;
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }
}

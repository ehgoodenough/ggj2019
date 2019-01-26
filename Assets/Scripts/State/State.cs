using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class State : MonoBehaviour
{
    protected StateMachine stateMachine;

    public void Enter()
    {
        enabled = true;
        DoEnter();
    }

    public void Exit()
    {
        enabled = false;
        DoExit();
    }

    void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        DoAwake();
    }

    void Start()
    {
        DoStart();
    }

    protected virtual void DoAwake() { }
    protected virtual void DoStart() { }
    protected virtual void DoEnter() { }
    protected virtual void DoExit() { }
    public virtual void DoUpdate() { }
}

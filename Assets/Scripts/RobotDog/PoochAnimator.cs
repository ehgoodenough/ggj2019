using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoochAnimator : MonoBehaviour
{
    Animator anim;

    private enum CurrentState
    {
        Idle,
        Trot,
        Bark,
        Run
    }

    private CurrentState currentState = CurrentState.Idle;

    public void Idle()
    {
        if (currentState != CurrentState.Idle)
        {
            anim.SetTrigger("Idle");
            currentState = CurrentState.Idle;
        }
    }

    public void Trot()
    {
        if (currentState != CurrentState.Trot)
        {
            anim.SetTrigger("Trot");
            currentState = CurrentState.Trot;
        }
    }

    public void Bark()
    {
        if (currentState != CurrentState.Bark)
        {
            anim.SetTrigger("Approach-Bark");
            currentState = CurrentState.Bark;
        }
    }

    public void Run()
    {
        if (currentState != CurrentState.Run)
        {
            anim.SetTrigger("Run");
            currentState = CurrentState.Run;
        }
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // for testing
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Idle();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Trot();
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Bark();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Run();
        }
    }
}

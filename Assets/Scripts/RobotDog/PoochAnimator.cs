using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoochAnimator : MonoBehaviour
{
    Animator anim;

    public void Idle()
    {
        anim.SetTrigger("Idle");
    }

    public void Trot()
    {
        anim.SetTrigger("Trot");
    }

    public void Bark()
    {
        anim.SetTrigger("Approach-Bark");
    }

    public void Run()
    {
        anim.SetTrigger("Run");
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

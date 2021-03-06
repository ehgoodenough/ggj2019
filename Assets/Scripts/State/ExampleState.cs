﻿using System.Collections.Generic;
using UnityEngine;

public class ExampleState : State
{
    protected override void DoAwake()
    {
        // Do something on Awake
    }

    protected override void DoStart()
    {
        // Do something on Start
    }

    protected override void DoEnter()
    {
        // Do something on the first frame when Entering the state
    }

    public override void DoUpdate()
    {
        // Do something on each frame in Update when state is active
    }

    protected override void DoExit()
    {
        // Do something on the last frame while Exiting the state
    }
}

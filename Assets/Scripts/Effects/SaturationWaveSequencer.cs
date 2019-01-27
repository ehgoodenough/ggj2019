﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturationWaveSequencer : MonoBehaviour
{
    public Material colorEffectMat;
    public float scanEffectRate = 2f;
    public float effectDistanceLimit = 20f;

    bool effectIsActive = false;

    [SerializeField]
    private float saturationIncrement = 1f/3f; // TODO: should be 1 / numItems

    [SubscribeGlobal]
    void ExitHome(LeaveHomeEvent e) // change to OnDestroy, if we're just changing scenes?
    {
        StopCoroutine(DoEffect());
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", 0);
    }

    private void Awake()
    {
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", GameProgress.homeSaturationLevel);
        colorEffectMat.SetFloat("_TargetSaturationLevel", GameProgress.homeSaturationLevel + saturationIncrement);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // For testing.
            IncreaseSaturationLevel();
        }
    }

    public void IncreaseSaturationLevel()
    {
        if (!effectIsActive)
        {
            colorEffectMat.SetFloat("_TargetSaturationLevel", GameProgress.homeSaturationLevel + saturationIncrement);
            StartCoroutine(DoEffect());
        }
    }

    IEnumerator DoEffect()
    {
        effectIsActive = true;
        float currentScanDistance = 0;

        while (currentScanDistance < effectDistanceLimit)
        {
            currentScanDistance += Time.deltaTime * scanEffectRate;
            colorEffectMat.SetFloat("_ScanDistance", currentScanDistance);
            yield return null;
        }

        GameProgress.homeSaturationLevel += saturationIncrement;
        colorEffectMat.SetFloat("_SaturationLevel", GameProgress.homeSaturationLevel);
        colorEffectMat.SetFloat("_ScanDistance", 0);
        Debug.Log("donezo");
        effectIsActive = false;
    }
}

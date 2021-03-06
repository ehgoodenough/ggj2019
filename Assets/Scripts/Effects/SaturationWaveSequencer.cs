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

    private void Awake()
    {
        // Debug.Log("SaturationWaveSequencer.Awake()");
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", GameProgress.homeSaturationLevel);
        colorEffectMat.SetFloat("_TargetSaturationLevel", GameProgress.homeSaturationLevel + saturationIncrement);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     // For testing.
        //     IncreaseSaturationLevel();
        // }
    }

    public void IncreaseSaturationLevel()
    {
        // Debug.Log("Increment Home Saturation Level in Game Progress");
        GameProgress.homeSaturationLevel += saturationIncrement;

        if (!effectIsActive)
        {
            colorEffectMat.SetFloat("_TargetSaturationLevel", GameProgress.homeSaturationLevel);
            StartCoroutine(DoEffect());
        }
    }

    void OnDestroy()
    {
        StopCoroutine(DoEffect());
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", 0);
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

        colorEffectMat.SetFloat("_SaturationLevel", GameProgress.homeSaturationLevel);
        colorEffectMat.SetFloat("_ScanDistance", 0);
        effectIsActive = false;
    }
}

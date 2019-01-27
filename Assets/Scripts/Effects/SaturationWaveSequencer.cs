﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturationWaveSequencer : MonoBehaviour
{
    public Material colorEffectMat;
    public float scanEffectRate = 2f;
    public float effectDistanceLimit = 20f;

    [SerializeField]
    private float currentHomeSaturationLevel;
    [SerializeField]
    private float saturationIncrement = .2f; // TODO: should be 1 / numItems

    void ExitHome()
    {
        StopCoroutine(DoEffect());
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", 0);
    }

    void EnterHome()
    {
        colorEffectMat.SetFloat("_ScanDistance", 0);
        colorEffectMat.SetFloat("_SaturationLevel", currentHomeSaturationLevel);
    }

    private void Awake()
    {
        colorEffectMat.SetFloat("_SaturationLevel", 0);
        colorEffectMat.SetFloat("_TargetSaturationLevel", saturationIncrement);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            IncreaseSaturationLevel();
        }
    }

    public void IncreaseSaturationLevel()
    {
        Debug.Log("increase that shit");
        colorEffectMat.SetFloat("_TargetSaturationLevel", currentHomeSaturationLevel + saturationIncrement);
        StartCoroutine(DoEffect());
    }

    IEnumerator DoEffect()
    {
        float currentScanDistance = 0;

        while (currentScanDistance < effectDistanceLimit)
        {
            currentScanDistance += Time.deltaTime * scanEffectRate;
            colorEffectMat.SetFloat("_ScanDistance", currentScanDistance);
            yield return null;
        }

        currentHomeSaturationLevel += saturationIncrement;
        colorEffectMat.SetFloat("_SaturationLevel", currentHomeSaturationLevel);
        colorEffectMat.SetFloat("_ScanDistance", 0);
    }
}

﻿using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    PlayerEnergy playerEnergy;
    public Image batteryFill;

    void Start()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
    }

    void Update()
    {
        if (playerEnergy)
        {
            batteryFill.fillAmount = playerEnergy.GetCurrentEnergy() / playerEnergy.GetCurrentMaxEnergy();
        }
    }
}

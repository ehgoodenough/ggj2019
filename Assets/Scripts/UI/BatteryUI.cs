using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    PlayerEnergy playerEnergy;
    Image batteryFill;

    void Start()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
        batteryFill = GetComponent<Image>();
    }

    void Update()
    {
        if (playerEnergy)
        {
            batteryFill.fillAmount = playerEnergy.GetCurrentEnergy() / playerEnergy.GetCurrentMaxEnergy();
        }
    }
}

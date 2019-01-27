using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCinematicStarter : MonoBehaviour
{
    private void Awake()
    {
        FindObjectOfType<CinematicPlayer>().PlayOpeningCinematicIfNecessary();
    }
}

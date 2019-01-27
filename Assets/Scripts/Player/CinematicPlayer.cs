using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CinematicPlayer : MonoBehaviour
{
    public bool openingCinematicPlayed = false;

    public void PlayOpeningCinematicIfNecessary()
    {
        if (!openingCinematicPlayed)
        {
            FindObjectOfType<PlayableDirector>().Play();
            openingCinematicPlayed = true;
        }
    }
}

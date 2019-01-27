using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CinematicPlayer : MonoBehaviour
{
    public PlayableAsset openingCinematicPlayable;
    public PlayableAsset closingCinematicPlayable;

    public bool openingCinematicPlayed = false;

    PlayableDirector director;

    private void Awake()
    {
        director = FindObjectOfType<PlayableDirector>();
    }

    public void PlayOpeningCinematicIfNecessary()
    {
        if (!openingCinematicPlayed)
        {
            director.playableAsset = openingCinematicPlayable;
            director.Play();
            openingCinematicPlayed = true;
        }
    }

    public void PlayFinalCinematic()
    {
        director.playableAsset = closingCinematicPlayable;
        director.Play();
    }
}

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

    [FMODUnity.EventRef]
    public string bootSoundEvent;

    PlayableDirector director;

    private void Awake()
    {
        director = FindObjectOfType<PlayableDirector>();
    }

    public void PlayOpeningCinematicIfNecessary()
    {
        if (!openingCinematicPlayed)
        {
            Debug.Log("PLAYING CINEMATIC");
            director.playableAsset = openingCinematicPlayable;
            director.Play();
            var bootSound = FMODUnity.RuntimeManager.CreateInstance(bootSoundEvent);
            bootSound.start();
            bootSound.release();
            openingCinematicPlayed = true;
        }
    }

    public void PlayFinalCinematic()
    {
        director.playableAsset = closingCinematicPlayable;
        director.Play();
    }
}

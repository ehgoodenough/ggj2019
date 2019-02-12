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
        // director = FindObjectOfType<PlayableDirector>();
        director = GameObject.Find("HUD").GetComponent<PlayableDirector>();
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

    /*
    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            PlayFinalCinematic();
        }
    }
    */

    public void PlayFinalCinematic()
    {
        Debug.Log("CinematicPlayer.PlayFinalCinematic()");
        //director.playableAsset = closingCinematicPlayable;
        //director.Play();
        GameObject.Find("friend_pieces").SetActive(false);
        // GameObject.Find("friend_whole").GetComponent<MeshRenderer>().enabled = true;
        GameObject.FindObjectOfType<FriendWhole>().Reveal();
        EventBus.PublishEvent(new FriendFullyAssembledEvent()); // This event kicks off the end credits
    }
}

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class FinalCinematicStarter : MonoBehaviour
{
    public bool playOnAwake = false;
    public bool playOnGameEnd = false;

    public FinalCinematicDynamicTimelineBinding dynamicBinding;

    public PlayableDirector finalCinematicDirector;

    public Camera finalCinematicCamera;
    public GameObject friendPiecesApart;
    public GameObject assemblyParticleEffect;
    public GameObject cinemachineVirtualCamera;

    private void Awake()
    {
        if (playOnAwake)
        {
            PlayFinalCinematic();
        }
        else
        {
            finalCinematicCamera.gameObject.SetActive(false);
            friendPiecesApart.SetActive(false);
            assemblyParticleEffect.SetActive(false);
            cinemachineVirtualCamera.SetActive(false);
        }
    }

    public void PlayFinalCinematic()
    {
        if (dynamicBinding) dynamicBinding.BindTimelineTracks();

        // Disable photo object before cinematic plays
        Photo photo = FindObjectOfType<Photo>();
        if (photo) photo.gameObject.SetActive(false);

        friendPiecesApart.SetActive(true);
        assemblyParticleEffect.SetActive(true);

        finalCinematicDirector.stopped += OnCinematicEnd;
        finalCinematicDirector.Play();
    }

    private void OnCinematicEnd(PlayableDirector director)
    {
        EventBus.PublishEvent(new FriendFullyAssembledEvent()); // This event kicks off the end credits
    }
}

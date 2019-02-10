using UnityEngine;
using UnityEngine.Playables;

public class FinalCinematicStarter : MonoBehaviour
{
    public bool playOnAwake;
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
        friendPiecesApart.SetActive(true);
        assemblyParticleEffect.SetActive(true);
        cinemachineVirtualCamera.SetActive(true);
        finalCinematicCamera.gameObject.SetActive(true);
        finalCinematicDirector.Play();
    }
}

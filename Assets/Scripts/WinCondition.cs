using System.Collections;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public SaturationWaveSequencer wave;

    private void Awake()
    {
        StartCoroutine(StartSaturationCinematic());
    }

    IEnumerator StartSaturationCinematic()
    {
        if (GameProgress.hasJustCompletedObjective)
        {
            EventBus.PublishEvent(new ObjectiveCompletedCutsceneStartEvent());
            GameProgress.hasJustCompletedObjective = false;

            yield return new WaitForSeconds(1.5f);

            Photo photo = GameObject.Find("Photo").GetComponent<Photo>(); // TODO: set up some singletons or use an event
            photo.HoldUpPhotoForCinematic();

            yield return new WaitForSeconds(Photo.HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS + .5f);

            wave.IncreaseSaturationLevel();
            var intro = FMODUnity.RuntimeManager.CreateInstance("event:/VO/Object_Deposited");
            intro.start();
            intro.release();

            yield return new WaitForSeconds(3);

            EventBus.PublishEvent(new ObjectiveCompletedCutsceneEndEvent());
        }

        if (GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Chair)
            && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Flowers)
            && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Art))
        {
            Debug.Log("WINNER IS YOU");
            EventBus.PublishEvent(new PlayerHasWonEvent());
            
            StartCoroutine(PlayFinalCinematic());
        }

        yield return null;
    }

    private IEnumerator PlayFinalCinematic()
    {
        FinalCinematicStarter newFinalCinematic = FindObjectOfType<FinalCinematicStarter>();
        if (newFinalCinematic && newFinalCinematic.playOnGameEnd)
        {
            newFinalCinematic.PlayFinalCinematic();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            FindObjectOfType<CinematicPlayer>().PlayFinalCinematic();
        }
    }
}

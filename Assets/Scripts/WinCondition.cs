using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject wave;

    private void Awake()
    {
        if (GameProgress.hasJustCompletedObjective == true) {
            GameProgress.hasJustCompletedObjective = false;
            // TODO: Show the photo first, then take it down, then run the cool effect.
            SaturationWaveSequencer w = wave.GetComponent(typeof(SaturationWaveSequencer)) as SaturationWaveSequencer;
            w.IncreaseSaturationLevel();

            var intro = FMODUnity.RuntimeManager.CreateInstance("event:/VO/Object_Deposited");
            intro.start();
            intro.release();
        }
        if(GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Chair)
        && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Flowers)
        && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Art)) {
            Debug.Log("WINNER IS YOU");
            EventBus.PublishEvent(new PlayerHasWonEvent());
            Invoke("PlayFinalCinematic", 4);
        }
    }

    public void PlayFinalCinematic()
    {
        FindObjectOfType<CinematicPlayer>().PlayFinalCinematic();
    }
}

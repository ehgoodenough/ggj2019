using System.Collections;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public SaturationWaveSequencer wave;
    public Transform waveOrigin;
    public Transform playerStartPosition;

    private void Awake()
    {
        StartCoroutine(StartSaturationCinematic());
    }

    IEnumerator StartSaturationCinematic()
    {
        if (GameProgress.hasJustCompletedObjective)
        {
            GameProgress.hasJustCompletedObjective = false;
            PlayerView playaView = FindObjectOfType<PlayerView>();
            playaView.RestrictView(true);

            PlayerMovement movement = FindObjectOfType<PlayerMovement>();
            GameObject playa = movement.gameObject;
            playa.transform.SetPositionAndRotation(playerStartPosition.position, playerStartPosition.rotation);
            playaView.ResetCamera();
            movement.RestrictMovement(true);

            yield return new WaitForSeconds(1.5f);

            Photo photo = GameObject.Find("Photo").GetComponent<Photo>(); // TODO: set up some singletons or use an event
            photo.HoldUpPhotoForCinematic();

            yield return new WaitForSeconds(Photo.HOW_LONG_TO_KEEP_IT_UP_FOR_CINEMATICS + .5f);

            wave.IncreaseSaturationLevel();
            var intro = FMODUnity.RuntimeManager.CreateInstance("event:/VO/Object_Deposited");
            intro.start();
            intro.release();

            yield return new WaitForSeconds(3);

            playaView.RestrictView(false);
            movement.RestrictMovement(false);
        }

        if (GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Chair)
            && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Flowers)
            && GameProgress.isObjectiveComplete(ObjectivePickupable.Type.Art))
        {
            Debug.Log("WINNER IS YOU");
            EventBus.PublishEvent(new PlayerHasWonEvent());
            Invoke("PlayFinalCinematic", 4);
        }

        yield return null;
    }

    public void PlayFinalCinematic()
    {
        FindObjectOfType<CinematicPlayer>().PlayFinalCinematic();
    }
}

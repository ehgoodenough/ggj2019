using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public GameObject virtualCamera;
    public float lookSpeed = 1f;
    public float verticalRotationMaxLimit = 89f;
    public float verticalRotationMinLimit = -89f;

    private float cameraRotation = 0;
    private bool isViewRestricted = true;

    void Start()
    {
        Debug.Assert(virtualCamera != null, "PlayerView requires a reference to the player Virtual Camera");

        cameraRotation = virtualCamera.transform.rotation.eulerAngles.x;

        EventBus.Subscribe<PlayerStartPositionEvent>(e => SpawnAtStartPosition(e.startTransform));
        EventBus.Subscribe<PhotoLoweredAtStartEvent>(e => RestrictView(false));
        EventBus.Subscribe<ObjectiveCompletedCutsceneStartEvent>(e => RestrictView(true));
        EventBus.Subscribe<ObjectiveCompletedCutsceneEndEvent>(e => RestrictView(false));
        EventBus.Subscribe<PlayerHasWonEvent>(e => RestrictView(true));
    }

    public void Look(Vector2 lookVector)
    {
        if (!isViewRestricted)
        {
            // Note that looking up / down only affects the camera rotation and should not affect entire player object
            cameraRotation = Mathf.Clamp(cameraRotation - lookVector.y * lookSpeed, verticalRotationMinLimit, verticalRotationMaxLimit);
            virtualCamera.transform.localRotation = Quaternion.AngleAxis(cameraRotation, Vector3.right);
        }
    }

    public void AddToYaw(float yaw)
    {
        if (!isViewRestricted)
        {
            // Note that looking left / right rotates the whole player object and therefore potentially affects movement direction
            transform.rotation *= Quaternion.AngleAxis(yaw * lookSpeed, Vector3.up);
        }
    }

    public void RestrictView(bool restrictView)
    {
        isViewRestricted = restrictView;
    }

    public void ResetCamera()
    {
        cameraRotation = 0;
        virtualCamera.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    private void SpawnAtStartPosition(Transform startTransform)
    {
        this.transform.rotation = startTransform.rotation;
        ResetCamera();
    }
}

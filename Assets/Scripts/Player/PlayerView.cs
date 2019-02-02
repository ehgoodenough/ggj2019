using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Camera playerCamera;
    public float lookSpeed = 1f;
    public float verticalRotationMaxLimit = 89f;
    public float verticalRotationMinLimit = -89f;

    private float cameraRotation = 0;
    private bool isViewRestricted = true;

    void Start()
    {
        Debug.Assert(playerCamera != null, "PlayerView requires a reference to the player Camera");

        cameraRotation = playerCamera.transform.rotation.eulerAngles.x;

        EventBus.Subscribe<PhotoLoweredAtStartEvent>(OnPhotoLoweredAtStartEvent);
        EventBus.Subscribe<PlayerHasWonEvent>(OnPlayerHasWonEvent);
    }

    public void Look(Vector2 lookVector)
    {
        if (!isViewRestricted)
        {
            // Note that looking up / down only affects the camera rotation and should not affect entire player object
            cameraRotation = Mathf.Clamp(cameraRotation - lookVector.y * lookSpeed, verticalRotationMinLimit, verticalRotationMaxLimit);
            playerCamera.transform.localRotation = Quaternion.AngleAxis(cameraRotation, Vector3.right);
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

    public void EnablePlayerCamera()
    {
        playerCamera.enabled = true;
    }

    public void DisablePlayerCamera()
    {
        playerCamera.enabled = false;
    }

    public bool IsPlayerCameraEnabled()
    {
        return playerCamera.enabled;
    }

    // TODO: Turn player in fluid motion toward robot friend
    private void OnPlayerHasWonEvent(PlayerHasWonEvent e)
    {
        RestrictView(true);
    }

    private void OnPhotoLoweredAtStartEvent(PhotoLoweredAtStartEvent e)
    {
        RestrictView(false);
    }
}

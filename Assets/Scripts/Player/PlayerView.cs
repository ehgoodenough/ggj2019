using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Camera playerCamera;
    public float lookSpeed = 1f;
    private float cameraRotation = 0;

    void Start()
    {
        Debug.Assert(playerCamera != null, "PlayerView requires a reference to the player Camera");

        cameraRotation = playerCamera.transform.rotation.eulerAngles.x;
    }

    public void Look(Vector2 lookVector)
    {
        // Note that looking up / down only affects the camera rotation and should not affect entire player object
        cameraRotation = Mathf.Clamp(cameraRotation - lookVector.y * lookSpeed, -89, 89);
        playerCamera.transform.localRotation = Quaternion.AngleAxis(cameraRotation, Vector3.right);
    }

    public void AddToYaw(float yaw)
    {
        // Note that looking left / right rotates the whole player object and therefore potentially affects movement direction
        transform.rotation *= Quaternion.AngleAxis(yaw * lookSpeed, Vector3.up);
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
}

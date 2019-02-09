using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerMovementInvisibleWallHandler : MonoBehaviour, ISlowingModifier
{
    public float minSlowerModifierValue = 0.35f; // Allow at least some movement so player doesn't get trapped at (nearly) zero
    public float checkForInvisibleWallsMaxDistance = 15f;
    public LayerMask InvisibleWallLayer;

    private PlayerMovement playerMovement;
    private CapsuleCollider playerCapsuleCollider;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
    }

    public float GetSlowingModifier()
    {
        if (playerMovement.GetCurrentMovementVectorInWorldSpace().sqrMagnitude == 0)
        {
            return 1f; // Skip raycast if not moving
        }

        Collider[] invisibleWallColliders = Physics.OverlapSphere(this.transform.position, checkForInvisibleWallsMaxDistance, InvisibleWallLayer, QueryTriggerInteraction.Ignore);
        if (invisibleWallColliders.Length == 0)
        {
            // Debug.Log("invisibleWallColliders.Length == 0");
            return 1f;
        }

        // Each Invisible Wall can have a different distance at which they start to slow the player
        // We need to find the Invisible Wall that has the maximum slowing power, i.e. lowest slowing modifier
        float minSlowingModifier = 1f;
        for (int i = 0; i < invisibleWallColliders.Length; i++)
        {
            InvisibleWall invisibleWall = invisibleWallColliders[i].GetComponent<InvisibleWall>();
            if (!invisibleWall || !invisibleWall.slowsToAStop) continue;

            float currentSlowingModifier = invisibleWall.GetNormalizedSlowingModifier(playerCapsuleCollider);
            minSlowingModifier = currentSlowingModifier < minSlowingModifier ? currentSlowingModifier : minSlowingModifier;
        }
        // Debug.Log("minSlowingModifier: " + minSlowingModifier);
        return Mathf.Clamp01(minSlowingModifier + minSlowerModifierValue); // Allow at least some movement so player doesn't get trapped at (nearly) zero
    }

    private void OnDrawGizmos()
    {
        // Invisible Walls
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, checkForInvisibleWallsMaxDistance);
    }
}

using UnityEngine;
using System.Collections;

public class InvisibleWall : MonoBehaviour
{
    public bool slowsToAStop = false;
    public float slowingRange = 3.5f;

    private Collider thisCollider;

    private void Awake()
    {
        thisCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Returns a value between 0 and 1 representing the degree to which this invsible wall should slow the player
    /// </summary>
    /// <returns></returns>
    public float GetNormalizedSlowingModifier(CapsuleCollider playerCollider)
    {
        if (!slowsToAStop || slowingRange == 0) return 1f;

        Vector3 playerColliderCenterInWorldSpace = playerCollider.transform.position + playerCollider.center;
        Vector3 closestPointOnThisCollider = thisCollider.ClosestPoint(playerColliderCenterInWorldSpace);
        Vector3 closestPointOnPlayerCollider = playerCollider.ClosestPoint(closestPointOnThisCollider);
        Vector3 thisColliderCenterInWorldSpace = thisCollider.bounds.center;

        Vector3 directionFromPlayerCenterToColliderCenter = (thisColliderCenterInWorldSpace - playerColliderCenterInWorldSpace).normalized;
        Vector3 directionFromPlayerClosestPointToColliderClosestPoint = (closestPointOnThisCollider - closestPointOnPlayerCollider).normalized;
        float dot = Vector3.Dot(directionFromPlayerCenterToColliderCenter, directionFromPlayerClosestPointToColliderClosestPoint);

        float signedDistance = dot * Vector3.Distance(closestPointOnThisCollider, closestPointOnPlayerCollider);
        return Mathf.Clamp01(signedDistance / slowingRange);
    }

    private float GetStartSlowingDistance()
    {
        return slowsToAStop ? slowingRange : 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (slowsToAStop)
        {
            Gizmos.color = Color.red;

            BoxCollider box = GetComponent<BoxCollider>();
            if (box)
            {
                Vector3 scaledSize = new Vector3(transform.lossyScale.x * box.size.x, transform.lossyScale.y * box.size.y, transform.lossyScale.z * box.size.z);
                Gizmos.DrawWireCube(this.transform.position + box.center, scaledSize + Vector3.one * slowingRange);
                return;
            }

            CapsuleCollider capsule = GetComponent<CapsuleCollider>();
            if (capsule)
            {
                float scaledRadius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * capsule.radius;
                Gizmos.DrawWireSphere(this.transform.position + capsule.center, scaledRadius + slowingRange * 2f);
                return;
            }

            SphereCollider sphere = GetComponent<SphereCollider>();
            if (sphere)
            {
                float scaledRadius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * sphere.radius;
                Gizmos.DrawWireSphere(this.transform.position + sphere.center, scaledRadius + slowingRange * 2f);
                return;
            }
        }
    }
}

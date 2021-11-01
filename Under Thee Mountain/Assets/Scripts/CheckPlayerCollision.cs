using UnityEngine;

public class CheckPlayerCollision : MonoBehaviour
{
    [SerializeField] protected LayerMask PlayerLayerMask;
    protected static bool CheckForObject(Collider collider, Vector3 direction, LayerMask passLayerMask, float rayMaxDist = Mathf.Infinity)
    {
        RaycastHit hit;
        Physics.BoxCast(
            collider.bounds.center,
            collider.bounds.size,
            direction,
            out hit,
            Quaternion.identity,
            rayMaxDist,
            passLayerMask
        );
        return hit.collider != null;
    }
}

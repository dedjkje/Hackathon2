using UnityEngine;

public class PlayerWallDetector : MonoBehaviour
{
    public LayerMask wallLayers;
    public float checkDistance = 0.5f;
    [SerializeField] private string currentWallTag;

    public string CurrentWallTag => currentWallTag;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, checkDistance, wallLayers))
        {
            currentWallTag = hit.collider.tag;
        }
        else
        {
            currentWallTag = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.up * checkDistance);
    }
}
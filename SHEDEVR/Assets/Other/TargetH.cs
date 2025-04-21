using UnityEngine;

public class TargetH : MonoBehaviour
{
    public float targetY;
    public LayerMask wallsLayer;
    ChangeGravity changeGravity;
    private float lowerHitY;
    private float upperHitY;
    private bool drawDebugRays = true;
    private void Start()
    {
        changeGravity = FindAnyObjectByType<ChangeGravity>();
    }
    private void Update()
    {
        



    }

    public float GetTargetY()
    {
        Vector3 currentPos = transform.position;

        RaycastHit downHit;
        if (Physics.Raycast(currentPos, Vector3.down, out downHit, Mathf.Infinity, wallsLayer))
        {
            lowerHitY = downHit.point.y;
        }
        else
        {
            lowerHitY = -8f;
        }


        RaycastHit upHit;
        if (Physics.Raycast(currentPos, Vector3.up, out upHit, Mathf.Infinity, wallsLayer))
        {
            upperHitY = upHit.point.y;
        }
        else
        {
            upperHitY = 8f;
        }
        float half1 = upperHitY / 2f;
        float half2 = lowerHitY / 2f;
        targetY = half1 + half2;

        return targetY;
    }
}
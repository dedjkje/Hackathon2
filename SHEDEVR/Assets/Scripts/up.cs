using UnityEngine;

public class up : MonoBehaviour
{
    [SerializeField] public GameObject Predict;
    [SerializeField] Camera playerCamera;
    public bool canCast;
    [SerializeField] LayerMask raycastMask;
    private ChangeGravity changeGravity;
    public bool animEnded;
    void Start()
    {
        changeGravity = GetComponent<ChangeGravity>();
    }
    public void animationStarted()
    {
        animEnded = false;
    }
    public void animationEnded()
    {
        animEnded = true;
    }
    
    void LateUpdate()
    {
        if (canCast && animEnded)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, raycastMask))
            {
                if (hit.collider.tag == "x wall 0")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(0f+changeGravity.x, 0f, 0f + changeGravity.z);
                }
                else if (hit.collider.tag == "x wall 180")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(180f + changeGravity.x, 0f, 0f + changeGravity.z);
                }
                else if (hit.collider.tag == "x wall 90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(90f + changeGravity.x, 0f, 0f + changeGravity.z);
                }
                else if (hit.collider.tag == "x wall -90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(-90f + changeGravity.x, 0f, 0f + changeGravity.z);
                }
                else if (hit.collider.tag == "z wall -90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(0f + changeGravity.x, 0f, -90f + changeGravity.z);
                }
                else if (hit.collider.tag == "z wall 90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.002f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.Euler(0f + changeGravity.x, 0f, 90f + changeGravity.z);
                }
                else
                {
                    Predict.transform.position = new Vector3(0, -1000f, 0);
                    
                }
            }
        }
        else
        {
            Predict.transform.position = new Vector3(0, -1000f, 0);
        }
    }

    public void Cast()
    {
        canCast = false;
    }
}

using UnityEngine;

public class up : MonoBehaviour
{
    [SerializeField] public GameObject Parent;
    [SerializeField] public GameObject Predict;
    [SerializeField] public GameObject Cilinder;
    private GameObject cilinder;
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
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
                }
                else if (hit.collider.tag == "x wall 180")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
                }
                else if (hit.collider.tag == "x wall 90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
                }
                else if (hit.collider.tag == "x wall -90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
                }
                else if (hit.collider.tag == "z wall -90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
                }
                else if (hit.collider.tag == "z wall 90")
                {
                    Predict.transform.position = new Vector3(
                        hit.point.x,
                        hit.point.y + 0.005f,
                        hit.point.z);
                    Predict.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, 0, 0);
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
        Destroy(cilinder);
        cilinder = Instantiate(Cilinder, Predict.transform.position, Predict.transform.rotation);
        cilinder.transform.parent = Parent.transform;
        canCast = false;
    }
}

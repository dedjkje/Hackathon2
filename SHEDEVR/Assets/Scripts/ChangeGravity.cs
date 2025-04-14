using System.Collections;
using UnityEngine;

public class ChangeGravity : MonoBehaviour
{

    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject[] hideUI;
    [SerializeField] Transform rotate;
    
    Abilities abilities;
    public string currentWall = "x wall 0";

    public float x = 0;
    public float z = 0;
    public float y = 0;

    public bool isRotating = false;
    void Start()
    {
        abilities = GetComponent<Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
        wall();
        if (abilities.currentAbility == Abilities.Ability.ChangeGravity)
        {
            if (wall() != "null" && wall() != currentWall && !isRotating && !abilities.changing)
            {
                foreach (GameObject ui in hideUI) ui.SetActive(true);

            }
            else
            {
                foreach (GameObject ui in hideUI) ui.SetActive(false);
            }
        }
        
    }

    string wall()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
            return hit.transform.tag;
        else
            return "null";
    }

    private IEnumerator CinemaRotate(float x, float y, float z, float duration)
    {
        isRotating = true;
        float elapsed = 0f;
        Quaternion initialRotation = rotate.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(x, 0, z) * initialRotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rotate.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotate.transform.rotation = targetRotation;
        isRotating = false;
    }

    public void change()
    {
        if (wall()[0] == 'z')
        {
            z = float.Parse(wall().Substring(7));
            y = -x;
            x = 0;
        }
        if (wall()[0] == 'x')
        {
            x = float.Parse(wall().Substring(7));
            z = 0;
        }
        StartCoroutine(CinemaRotate(x, 0, z, 1));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravity : MonoBehaviour
{
    [System.Serializable]
    public class RotationRule
    {
        public string currentWall;
        public string targetWall;
        public Vector3 eulerDelta;
    }
    [SerializeField] public FirstPersonController firstPersonController;
    [SerializeField] public GameObject Predict;
    public AudioSource audioSource;
    public AudioClip gravityFlipSound;
    public float soundDuration = 3f; // ����� ������������ �����
    public float fadeOutDuration = 0.5f; // ������������ ���������
    public bool showUI;
    public GameObject currentWallGO;
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject[] hideUI;
    [SerializeField] public Transform rotate;
    [SerializeField] LayerMask wallLayer;

    private List<RotationRule> rotationRules = new List<RotationRule>();
    private Abilities abilities;
    public string currentWall = "x wall 0";
    public bool isRotating = false;
    public float x;
    public float y;
    public float z;
    public GameObject[] upableObjects;
    public string targetWall;
    public Animator handAnimator;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 0.05f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    public MusicController musicController;

    public float targetHeight = 10f; // ������� ������
    public float effectDuration = 4f; // ������������ �������
    public float liftSpeed = 7f; // �������� �������
    void Start()
    {
        abilities = GetComponent<Abilities>();
        InitializeRotationRules();
        originalPosition = playerCamera.transform.localPosition;
    }
    public void StartShake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(Shake());
    }
    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            playerCamera.transform.localPosition = originalPosition;
        }
    }
    private IEnumerator Shake()
    {
        while (true)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            playerCamera.transform.localPosition = originalPosition + randomOffset;
            yield return new WaitForSeconds(shakeFrequency);
        }
    }

    void InitializeRotationRules()
    {
        rotationRules = new List<RotationRule>
        {
            // X Wall 0
            CreateRule("x wall 0", "x wall 90", new Vector3(-90, 0, 0)),
            CreateRule("x wall 0", "x wall -90", new Vector3(90, 0, 0)),
            CreateRule("x wall 0", "x wall 180", new Vector3(-180, 0, 0)),
            CreateRule("x wall 0", "z wall -90", new Vector3(0, 0, -90)),
            CreateRule("x wall 0", "z wall 90", new Vector3(0, 0, 90)),

            // X Wall 90
            CreateRule("x wall 90", "x wall 0", new Vector3(90, 0, 0)),
            CreateRule("x wall 90", "x wall -90", new Vector3(-180, 0, 0)),
            CreateRule("x wall 90", "x wall 180", new Vector3(-90, 0, 0)),
            CreateRule("x wall 90", "z wall -90", new Vector3(90, 90, -90)),
            CreateRule("x wall 90", "z wall 90", new Vector3(90, -90, 90)),

            // X Wall -90
            CreateRule("x wall -90", "x wall 0", new Vector3(-90, 0, 0)),
            CreateRule("x wall -90", "x wall 90", new Vector3(-180, 0, 0)),
            CreateRule("x wall -90", "x wall 180", new Vector3(90, 0, 0)),
            CreateRule("x wall -90", "z wall -90", new Vector3(-90, -90, -90)),
            CreateRule("x wall -90", "z wall 90", new Vector3(-90, 90, 90)),

            // X Wall 180
            CreateRule("x wall 180", "x wall 0", new Vector3(-180, 0, 0)),
            CreateRule("x wall 180", "x wall 90", new Vector3(-90, 0, 0)),
            CreateRule("x wall 180", "x wall -90", new Vector3(90, 0, 0)),
            CreateRule("x wall 180", "z wall -90", new Vector3(0, 0, 90)),
            CreateRule("x wall 180", "z wall 90", new Vector3(0, 0, -90)),

            // Z Wall -90
            CreateRule("z wall -90", "x wall 0", new Vector3(0, 0, 90)),
            CreateRule("z wall -90", "x wall -90", new Vector3(90, 0, 0)),
            CreateRule("z wall -90", "x wall 90", new Vector3(-90, 0, 0)),
            CreateRule("z wall -90", "x wall 180", new Vector3(0, 0, -90)),
            CreateRule("z wall -90", "z wall 90", new Vector3(0, 0, -180)),

            // Z Wall 90
            CreateRule("z wall 90", "x wall 0", new Vector3(0, 0, -90)),
            CreateRule("z wall 90", "x wall -90", new Vector3(90, 0, 0)),
            CreateRule("z wall 90", "x wall 90", new Vector3(-90, 0, 0)),
            CreateRule("z wall 90", "x wall 180", new Vector3(0, 0, 90)),
            CreateRule("z wall 90", "z wall -90", new Vector3(-180, 0, 0))
        };
    }

    RotationRule CreateRule(string current, string target, Vector3 delta)
    {
        return new RotationRule { currentWall = current, targetWall = target, eulerDelta = delta };
    }
    private void LateUpdate()
    {
        string hitWall = GetWallTag();
        showUI = hitWall != "null" &&
                abilities.currentAbility == Abilities.Ability.ChangeGravity &&
                        firstPersonController.isGrounded &&
                        hitWall != currentWall &&
                        !isRotating &&
                        !abilities.changing &&
                        rotationRules.Exists(r => r.currentWall == currentWall && r.targetWall == hitWall);
        
        if (abilities.currentAbility == Abilities.Ability.ChangeGravity)
        {
            showUI = hitWall != "null" &&
                abilities.currentAbility == Abilities.Ability.ChangeGravity &&
                        firstPersonController.isGrounded &&
                        hitWall != currentWall &&
                        !isRotating &&
                        !abilities.changing &&
                        rotationRules.Exists(r => r.currentWall == currentWall && r.targetWall == hitWall);
            
            foreach (GameObject go in hideUI) go.SetActive(showUI);
        }
    }
    void Update()
    {
        x = rotate.eulerAngles.x;
        y = rotate.eulerAngles.y;
        z = rotate.eulerAngles.z;
        

        

        if (showUI)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, wallLayer))
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

    string GetWallTag()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, wallLayer))
        {
            currentWallGO = hit.transform.gameObject;
        }
        return Physics.Raycast(ray, out RaycastHit hhit, 100f, wallLayer) ? hhit.transform.tag : "null";
    }

    public void Change()
    {
        targetWall = GetWallTag();
        if (targetWall == "null" || targetWall == currentWall || isRotating) return;

        RotationRule rule = rotationRules.Find(r =>
            r.currentWall == currentWall &&
            r.targetWall == targetWall);

        if (rule != null)
        {
            StartCoroutine(PerformRotation(rule.eulerDelta));
            currentWall = targetWall;
        }
    }
    private IEnumerator PlaySoundWithFade()
    {
        // ������������� ����
        audioSource.clip = gravityFlipSound;
        audioSource.Play();

        // ��� �������� ����� ������������ (3s - 0.5s)
        yield return new WaitForSeconds(soundDuration - fadeOutDuration);

        // ������� ��������� ��������� 0.5 ������
        float startVolume = audioSource.volume;
        float fadeTimer = 0f;

        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, fadeTimer / fadeOutDuration);
            yield return null;
        }

        // ������������� ����
        audioSource.Stop();
        audioSource.volume = startVolume; // ��������������� ���������
    }
    private IEnumerator PerformRotation(Vector3 delta)
    {
        Debug.Log($"Start: {rotate.rotation.eulerAngles} + Delta: {delta}");
        isRotating = true;
        handAnimator.SetBool("change grav", true);
        StartCoroutine(GravityEffectRoutine());
        StartShake();
        StartCoroutine(PlaySoundWithFade());
        musicController.TemporaryMute(3.5f);
        yield return new WaitForSeconds(1f);
        float elapsed = 0f;
        foreach (GameObject go in upableObjects)
        {
            Vector3 distance = new Vector3(
                rotate.position.x - go.transform.position.x,
                rotate.position.y - go.transform.position.y,
                rotate.position.z - go.transform.position.z);
            go.GetComponent<Rigidbody>().linearVelocity = distance;

        }
        // ���� ������� ���� � ���������� ������
        Vector3 startEuler = rotate.rotation.eulerAngles;
        Vector3 targetEuler = startEuler + delta;

        // ������������� ����������� ���� � �������� [0, 360)
        targetEuler.x = Mathf.Repeat(targetEuler.x, 360f);
        targetEuler.y = Mathf.Repeat(targetEuler.y, 360f);
        targetEuler.z = Mathf.Repeat(targetEuler.z, 360f);

        Quaternion startRotation = rotate.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        while (elapsed < 3f)
        {
            rotate.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                elapsed / 3f
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotate.rotation = targetRotation;
        Debug.Log($"Result: {targetRotation.eulerAngles}");
        isRotating = false;
        StopShake();

    }

    private IEnumerator GravityEffectRoutine()
    {
        // Сохраняем оригинальные XZ позиции
        Vector3[] originalXZ = new Vector3[upableObjects.Length];
        for (int i = 0; i < upableObjects.Length; i++)
        {
            if (upableObjects[i] != null)
            {
                Vector3 pos = upableObjects[i].transform.position;
                originalXZ[i] = new Vector3(pos.x, 0, pos.z);
            }
        }

        // Отключаем гравитацию и добавляем случайное вращение
        foreach (GameObject obj in upableObjects)
        {
            if (obj != null && obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;

                // Добавляем случайный вращательный импульс
                rb.angularVelocity = new Vector3(
                    Random.Range(-2f, 2f),  // Вращение вокруг X
                    Random.Range(-1f, 1f),  // Вращение вокруг Y (меньше для более естественного вида)
                    Random.Range(-2f, 2f)   // Вращение вокруг Z
                );
            }
        }

        // Плавный подъем (только Y) с сохранением вращения
        float timer = 0f;
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < upableObjects.Length; i++)
            {
                if (upableObjects[i] != null && upableObjects[i].GetComponent<UpFlag>().holder == null)
                {
                    Vector3 currentPos = upableObjects[i].transform.position;
                    Vector3 newPos = new Vector3(
                        originalXZ[i].x,
                        Mathf.Lerp(currentPos.y, upableObjects[i].GetComponent<TargetH>().GetTargetY(), liftSpeed * Time.deltaTime),
                        originalXZ[i].z
                    );

                    if (upableObjects[i].TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.MovePosition(newPos);
                        // Можно добавить небольшое замедление вращения со временем
                        rb.angularVelocity *= 0.98f;
                    }
                    else
                    {
                        upableObjects[i].transform.position = newPos;
                    }
                }
            }
            yield return null;
        }

        // Включаем гравитацию для естественного падения
        foreach (GameObject obj in upableObjects)
        {
            if (obj != null && obj.TryGetComponent<Rigidbody>(out var rb) && obj.GetComponent<UpFlag>().holder == null)
            {
                rb.useGravity = true;
                // Можно добавить дополнительное вращение при падении
                rb.angularVelocity += new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                ) * 0.5f;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }
    public void endAnimation()
    {
        handAnimator.SetBool("change grav", false);
    }
}
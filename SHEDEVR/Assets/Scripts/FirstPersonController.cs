using System.Security.Cryptography;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [Header("Параметры персонажа")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float gravityPower;
    [SerializeField] public float hp;

    [Header("Джойстик")]
    [SerializeField] Joystick joystick;

    [Header("Камера")]
    [SerializeField] float cameraSensivity;
    [SerializeField] Transform cameraTransform;

    [Header("Цилиндр")]
    [SerializeField] float cilinderPower;
    [SerializeField] AudioClip cilinderEnter;
    private AudioSource source;
    public float gravityForce;
    [HideInInspector] public Vector3 moveVector;
    [HideInInspector] public Vector3 moveDelta;
    public bool isGrounded;
    private CharacterController characterController;
    private int rightFingerId;
    private float halfScreenWidth;
    private Vector2 lookInput;
    private float cameraPitch;
    private Vector3 previousPosition;
    private Vector3 holder;
    private Vector3 deltaHolder;
    private GameObject prevCilinder;
    private ChangeGravity changeGravity;
    private float soundInterval = 2f;
    private float lastPlayTime = 0f;
    private Collider ccollider;
    private Rigidbody rb;
    private bool canRotateCamera = true;
    private MusicController musicController;
    private up addGravity;
    private Abilities abilities;
    private CastPull pull;
    [SerializeField] AudioClip fall;
    public bool isDead = false;
    public bool changing = false;

    void Start()
    {
        musicController = transform.Find("Music").gameObject.GetComponent<MusicController>();
        source = GetComponent<AudioSource>();
        changeGravity = transform.Find("Hand").gameObject.GetComponent<ChangeGravity>();
        pull = transform.Find("Hand").gameObject.GetComponent<CastPull>();
        abilities = transform.Find("Hand").gameObject.GetComponent<Abilities>();
        addGravity = transform.Find("Hand").gameObject.GetComponent<up>();
        moveDelta = transform.position;
        previousPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        rightFingerId = -1;
        halfScreenWidth = Screen.width / 2;
    }

    private void FixedUpdate()
    {
        if (characterController != null && !isDead)
        {
            if (!abilities.changingNORMAL)
            {

                MovePlayer();

            }
            GamingGravity();
            AddGravity();

            if (rightFingerId != -1 && canRotateCamera)
            {
                LookAround();
            }

            moveDelta = transform.position - previousPosition;
            previousPosition = transform.position;
        }
    }

    void Update()
    {
        if(hp <= 0)
        {
            Death();    
        }
        changing = abilities.changingNORMAL;
        if (characterController != null && !isDead)
        {
            GetTouchInput();
            isGrounded = characterController.isGrounded;
            if (prevCilinder == null)
            {
                holder = Vector3.zero;
                deltaHolder = Vector3.zero;
            }
            if (changeGravity.isRotating && prevCilinder != null && holder != Vector3.zero)
            {
                // potom
            }
        }
    }

    private void AddGravity()
    {
        if (holder != Vector3.zero)
        {
            deltaHolder = new Vector3(holder.x - transform.position.x, 0, holder.z - transform.position.z);
            if (Vector3.Distance(transform.position, holder) > 1f)
            {
                gravityForce = (holder.y - transform.position.y) / cilinderPower;
            }
            else
            {
                gravityForce = 0;
            }
        }
    }
    private void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        rightFingerId = t.fingerId;
                    }
                    break;  
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                    }
                    break;
                case TouchPhase.Moved:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensivity * Time.deltaTime;
                    }
                    break;
                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    private void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        transform.Rotate(transform.up, lookInput.x);
    }

    private void MovePlayer()
    {
        moveVector = Vector2.zero;
        if (joystick.background.gameObject.activeSelf)
        {
            moveVector.x = joystick.Horizontal;
            moveVector.z = joystick.Vertical;
        }
        moveVector.y = gravityForce;

        moveVector = transform.right * moveVector.x + transform.forward * moveVector.z + transform.up * moveVector.y + deltaHolder / (cilinderPower * 2);

        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }

    private void GamingGravity()
    {
        if (changeGravity.isRotating && holder == Vector3.zero)
        {
            if (transform.position.y < 0f)
            {
                gravityForce = 2f;
            }
            else
            {
                gravityForce = 0f;
            }
        }
        else if (changeGravity.isRotating && holder != Vector3.zero)
        {
            Debug.Log("VOZNYAA");
            characterController.Move(deltaHolder * Time.deltaTime * 100);
        }
        else if (!characterController.isGrounded && holder == Vector3.zero)
        {
            gravityForce -= gravityPower * Time.deltaTime;
        }
        else if (holder == Vector3.zero)
        {
            gravityForce = -1f;
        }
    }

    public void onClickJump()
    {
        if (characterController.isGrounded)
        {
            gravityForce = jumpPower;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "cilinder")
        {
            if (Time.time - lastPlayTime >= soundInterval)
            {
                source.clip = cilinderEnter;
                source.Play();
                lastPlayTime = Time.time;
                source.clip = cilinderEnter;
            }
            holder = other.gameObject.transform.parent.transform.Find("Holder").gameObject.transform.position;
            prevCilinder = other.gameObject;
        }
        if (other.tag == "death")
        {
            Death();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "cilinder")
        {
            holder = Vector3.zero;
            deltaHolder = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (ccollider != null)
        {
            if (collision.gameObject.layer == 8)
            {
                source.clip = fall;
                source.Play();
            }
        }
    }
    private void Death()
    {
        canvas.enabled = false;
        if (ccollider == null && rb == null)
        {
            ccollider = gameObject.AddComponent<CapsuleCollider>();
            rb = gameObject.AddComponent<Rigidbody>();
        }
        Transform hand = transform.Find("Hand");
        hand.gameObject.GetComponent<Abilities>().enabled = false;
        hand.gameObject.GetComponent<ChangeGravity>().enabled = false;
        hand.gameObject.GetComponent<up>().enabled = false;
        canRotateCamera = false;
        Destroy(addGravity.cilinder);
        musicController.TemporaryMute(10f);
        abilities.currentAbility = Abilities.Ability.PullObject;
        Destroy(addGravity.Predict);
        pull.enabled = false;
        abilities.enabled = false;
        isDead = true;
        //source.clip = fall;
        //source.Play();
        // reload
    }
}

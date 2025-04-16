using System.Security.Cryptography;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
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
    public float gravityForce;
    [HideInInspector] public Vector3 moveVector;
    [HideInInspector] public Vector3 moveDelta;
    [HideInInspector] public bool isGrounded;
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

    void Start()
    {
        changeGravity = transform.Find("Hand").gameObject.GetComponent<ChangeGravity>();
        moveDelta = transform.position;
        previousPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        rightFingerId = -1;
        halfScreenWidth = Screen.width / 2;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        GamingGravity();
        AddGravity();

        if (rightFingerId != -1)
        {
            LookAround();
        }

        moveDelta = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    void Update()
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
            // хз пока как это пофиксить
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
        moveVector.x = joystick.Horizontal;
        moveVector.z = joystick.Vertical;
        moveVector.y = gravityForce;

        moveVector = transform.right * moveVector.x + transform.forward * moveVector.z + transform.up * moveVector.y + deltaHolder / (cilinderPower * 2);

        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }

    private void GamingGravity()
    {
        if (changeGravity.isRotating && holder == Vector3.zero)
        {
            if (transform.position.y < 10f)
            {
                gravityForce = 4f;
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
            holder = other.gameObject.transform.parent.transform.Find("Holder").gameObject.transform.position;
            prevCilinder = other.gameObject;
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
}

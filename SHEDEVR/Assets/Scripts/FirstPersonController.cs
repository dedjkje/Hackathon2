using System.Security.Cryptography;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Параметры персонажа")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float gravityPower;

    [Header("Джойстик")]
    [SerializeField] Joystick joystick;

    [Header("Камера")]
    [SerializeField] float cameraSensivity;
    [SerializeField] Transform cameraTransform;

    private float gravityForce;
    [HideInInspector] public Vector3 moveVector;
    [HideInInspector] public Vector3 moveDelta;
    [HideInInspector] public bool isGrounded;
    private CharacterController characterController;
    private int rightFingerId;
    private float halfScreenWidth;
    private Vector2 lookInput;
    private float cameraPitch;
    private Vector3 previousPosition;

    public float hp = 100f;

    void Start()
    {
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

        moveVector = transform.right * moveVector.x + transform.forward * moveVector.z + transform.up * moveVector.y;

        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }

    private void GamingGravity()
    {
        if(!characterController.isGrounded)
        {
            gravityForce -= gravityPower * Time.deltaTime;
        }
        else
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
}

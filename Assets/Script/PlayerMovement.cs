using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("StartPoint")]
    [SerializeField] private Transform startPoint;
    [Header("Input")]
    [SerializeField] private float inputSensitivity;
    private PlayerControls playerControls;

    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lrSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    private Vector3 desireDirection;

    [Header("Turn")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnCooldown;

    [Header("LayerMask")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask obstacles;


    private Rigidbody playerRig;
    private RaycastHit groundHit;
    private Transform groundDetector;
    private bool isGrounded;
    private bool canJump;
    private bool canTurn;
    private int turn;
    private Quaternion tempRotation;

    #region OLD
    /*
    public void OnJump(InputValue n)
    {
        if (n.Get<Vector2>().magnitude < inputSensitivity)
            Debug.Log("Jump!");
    }
    public void OnTurnLeft(InputValue n)
    {
        if(n.Get<float>() > inputSensitivity)
           Debug.Log("Turn left");
    }
    public void OnTurnRight(InputValue n)
    {
        if (n.Get<float>() > inputSensitivity)
            Debug.Log("Turn right");
    }
    public void OnSlide(InputValue n)
    {
        if (n.Get<float>() > inputSensitivity)
            Debug.Log("Slide");
    }
    */
    #endregion

    #region MONO
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    #endregion
    private void Start()
    {
        turn = 0;
        canTurn = true;
        canJump = true;
        playerRig = GetComponent<Rigidbody>();
        groundDetector = transform.Find("Misc/GroundDetector");
        isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, .3f, ground);
        desireDirection = Vector3.forward;
    }
    private void FixedUpdate()
    {
        if (PlayerStatus.moveable)
        {
            Movement();
        }
    }
    void Update()
    {
        isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, .1f, ground);

        // Jump input
        if (playerControls.PhoneControl.Jump.ReadValue<float>() > inputSensitivity && canJump)
        {
            Jump();
        }

        // Turn right input.
        if (playerControls.PhoneControl.Turn.ReadValue<float>() > inputSensitivity
            && playerControls.PhoneControl.Jump.ReadValue<float>() < playerControls.PhoneControl.Turn.ReadValue<float>()
            && canTurn)
        {
            tempRotation = transform.localRotation * Quaternion.AngleAxis(117, Vector3.up);
            turn = 1;
            canTurn = false;

            CancelInvoke(nameof(ResetTurn));
            Invoke(nameof(ResetTurn), turnCooldown);
        }

        // Turn left input.
        if (playerControls.PhoneControl.Turn.ReadValue<float>() < -inputSensitivity
            && -playerControls.PhoneControl.Jump.ReadValue<float>() > playerControls.PhoneControl.Turn.ReadValue<float>()
            && canTurn)
        {
            tempRotation = transform.localRotation * Quaternion.AngleAxis(-90, Vector3.up);
            turn = -1;
            canTurn = false;

            CancelInvoke(nameof(ResetTurn));
            Invoke(nameof(ResetTurn), turnCooldown);
        }

        #region TurnSlerp
        if (turn == 1)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, tempRotation, turnSpeed * Time.deltaTime);
            if(Mathf.Abs(transform.localRotation.eulerAngles.y - tempRotation.eulerAngles.y) < 0.5f)
            {
                transform.localRotation = tempRotation;
                turn = 0;
            }
        }
        if (turn == -1)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, tempRotation, turnSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.localRotation.eulerAngles.y - tempRotation.eulerAngles.y) < 0.5f)
            {
                transform.localRotation = tempRotation;
                turn = 0;
            }
        }
        #endregion

        // Reset when fall.
        if (transform.position.y < -10f)
        {
            ResetPosition();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ResetPosition();
        }
    }
    void ResetTurn()
    {
        if (turn != 0)
            Debug.LogError("轉向冷卻時間太快了！請增加轉向冷卻時間或增加轉向速度。The turn cooldown is too quick! Please increase turn cooldown or increase turn speed.");
        canTurn = true;
    }
    void ResetPosition()
    {   
        transform.position = startPoint.position;
        transform.localRotation = startPoint.localRotation;
    }
    void Jump()
    {
        canJump = false;
        canTurn = false;
        playerRig.velocity = new Vector3(playerRig.velocity.x, jumpForce, playerRig.velocity.z);
        Invoke(nameof(ResetTurn), .3f);
        CancelInvoke(nameof(ResetJump));
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    void ResetJump()
    {
        if(!isGrounded)
        {
            CancelInvoke(nameof(ResetJump));
            Invoke(nameof(ResetJump), .5f);
            return;
        }
        
        canJump = true;
    }
    void Movement()
    {
        if (Physics.Raycast(groundDetector.position, Vector3.down, out groundHit, 10f, ground))
        {
            desireDirection = Vector3.ProjectOnPlane(Vector3.forward, groundHit.normal);
            
        }
        
        transform.Translate(moveSpeed * Time.deltaTime * desireDirection);

        // Debug.Log(playerControls.PhoneControl.Move.ReadValue<float>());

        transform.Translate(lrSpeed * Input.acceleration.x * Time.deltaTime * Vector3.right);
    }

    public float GetGroundRotation()
    {
        return groundHit.collider.gameObject.transform.eulerAngles.y;
    }
    public Vector2 GetGroundPos()
    {
        return new Vector2(groundHit.collider.gameObject.transform.position.x, groundHit.collider.gameObject.transform.position.z);
    }
}

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
    [HideInInspector] public float turnRightAngle;
    [HideInInspector] public float turnLeftAngle;
    [HideInInspector] public float turnStraightAngle;
    [HideInInspector] public bool goIncline;

    [Header("LayerMask")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask obstacles;


    private Rigidbody playerRig;
    private RaycastHit groundHit;
    private Transform groundDetector;
    private bool isGrounded;
    private bool canJump;
    public bool canTurn;
    private bool turnSlerp;
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
        turnRightAngle = 90;
        turnLeftAngle = 90;
        turnStraightAngle = 180;
        goIncline = false;
        turnSlerp = false;
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
            if(!PlayerStatus.inIntersection)
            {
                Debug.Log("Try to turn when wasn't in intersection.");
            }
            else
            {
                tempRotation = transform.localRotation * Quaternion.AngleAxis(turnRightAngle, Vector3.up);
                turnSlerp = true;
                canTurn = false;
                PlayerStatus.inIntersection = false;

                CancelInvoke(nameof(ResetTurn));
                Invoke(nameof(ResetTurn), turnCooldown);
            }
        }

        // Turn left input.
        if (playerControls.PhoneControl.Turn.ReadValue<float>() < -inputSensitivity
            && -playerControls.PhoneControl.Jump.ReadValue<float>() > playerControls.PhoneControl.Turn.ReadValue<float>()
            && canTurn)
        {
            if (!PlayerStatus.inIntersection)
            {
                Debug.Log("Try to turn when wasn't in intersection.");
            }
            else
            {
                tempRotation = transform.localRotation * Quaternion.AngleAxis(turnLeftAngle, Vector3.up);
                turnSlerp = true;
                canTurn = false;
                PlayerStatus.inIntersection = false;

                CancelInvoke(nameof(ResetTurn));
                Invoke(nameof(ResetTurn), turnCooldown);
            }
        }
        if (goIncline)
        {
            tempRotation = transform.localRotation * Quaternion.AngleAxis(turnStraightAngle, Vector3.up);
            turnSlerp = true;
            canTurn = false;
            goIncline = false;

            CancelInvoke(nameof(ResetTurn));
            Invoke(nameof(ResetTurn), turnCooldown);
        }

        #region TurnSlerp
        if (turnSlerp)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, tempRotation, turnSpeed * Time.deltaTime);
            if(Mathf.Abs(transform.localRotation.eulerAngles.y - tempRotation.eulerAngles.y) < 0.5f)
            {
                transform.localRotation = tempRotation;
                turnSlerp = false;
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ResetPosition();
        }
    }

    void ResetTurn()
    {
        if (turnSlerp)
            Debug.LogError("轉向冷卻時間太快了！請增加轉向冷卻時間或增加轉向速度。The turn cooldown is too quick! Please increase turn cooldown or increase turn speed.");
        canTurn = true;
        turnLeftAngle = 90;
        turnRightAngle = 90;
        turnStraightAngle = 180;

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

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("StartPoint")]
    [SerializeField] private Transform startPoint;
    [Header("Input")]
    [SerializeField] private float inputSensitivity;
    private PlayerControls playerControls;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lrSpeed;
    [SerializeField] private float jumpForce;
    private Vector3 desireDirection;

    [Header("LayerMask")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask obstacles;


    private Rigidbody playerRig;
    private RaycastHit groundHit;
    private Transform groundDetector;
    private bool isGrounded;
    private bool canJump;

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

        if(playerControls.PhoneControl.Jump.ReadValue<float>() != 0 && canJump)
        {
            Jump();
        }
        if(playerRig.velocity.y < 0 && isGrounded && !canJump)
        {
            canJump = true;
        }
        if(transform.position.y < -10f)
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
    void ResetPosition()
    {   
        transform.position = startPoint.position;
        transform.localRotation = startPoint.localRotation;
    }
    void Jump()
    {
        canJump = false;
        playerRig.velocity = new Vector3(playerRig.velocity.x, jumpForce, playerRig.velocity.z);
    }
    void Movement()
    {
        if (Physics.Raycast(groundDetector.position, Vector3.down, out groundHit, 10f, ground))
        {
            desireDirection = Vector3.ProjectOnPlane(Vector3.forward, groundHit.normal);
        }
        transform.Translate(moveSpeed * Time.deltaTime * desireDirection);


        transform.Translate(lrSpeed * playerControls.PhoneControl.Move.ReadValue<float>() * Time.deltaTime * Vector3.right);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    Vector3 desireDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lrSpeed;
    [SerializeField] private LayerMask ground;
    private RaycastHit groundHit;
    private Transform groundDetector;
    private bool isGrounded;
    private void Start()
    {
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

    }
    void Movement()
    {
        if (Physics.Raycast(groundDetector.position, Vector3.down, out groundHit, 10f, ground))
        {
            desireDirection = Vector3.ProjectOnPlane(Vector3.forward, groundHit.normal);
        }
        transform.Translate(moveSpeed * Time.deltaTime * desireDirection);

        transform.Translate(lrSpeed * AccelerometerManager.instance.GetAcceleration().x * Time.deltaTime * Vector3.right);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    void Update()
    {
        if (PlayerStatus.moveable)
        {
            Movement();
        }
    }
    void Movement()
    {
        transform.Translate(moveSpeed * Time.deltaTime * transform.forward);
    }
}

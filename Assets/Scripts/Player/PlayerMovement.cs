using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D playerRb;
    private float moveHor;
    private float moveVer;
    private float moveLimit = 0.7f;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveHor = Input.GetAxisRaw("Horizontal");
        moveVer = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (moveHor != 0 && moveVer != 0)
        {
            moveHor *= moveLimit;
            moveVer *= moveLimit;
        }

        playerRb.velocity = new Vector2(moveHor * speed, moveVer * speed);
    }

}

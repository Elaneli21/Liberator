using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRen;

    private float moveHor;
    private float moveVer;
    private float moveLimit = 0.7f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //receive input for movement
        moveHor = Input.GetAxisRaw("Horizontal");
        moveVer = Input.GetAxisRaw("Vertical");

        //flip sprite according to move direction
        if (moveHor != 0) spriteRen.flipX = (moveHor < 0);

        //play animation when doing action
        if (moveHor != 0 || moveVer != 0) anim.Play("Player_Run");
        else if (moveHor == 0 || moveVer == 0) anim.Play("Player_Idle");
    }

    void FixedUpdate()
    {
        //limit sideway movement speed
        if (moveHor != 0 && moveVer != 0)
        {
            moveHor *= moveLimit;
            moveVer *= moveLimit;
        }

        //implement movement
        rigid.velocity = new Vector2(moveHor * speed, moveVer * speed);
    }

}

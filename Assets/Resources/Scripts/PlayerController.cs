using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    [SerializeField] float wallJumpTimer = 0.5f; //Time after walljump to block input?
    private float timer = 0.0f; //tracks elapsed time

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    //FROM: https://unity3d.com/learn/tutorials/topics/2d-game-creation/player-controller-script?playlist=17093

    // Use this for initialization
    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity() //Called every frame by base class: PhysicsObject
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        //if (Input.GetButtonDown("Jump") && grounded)
        if (Input.GetButtonDown("Jump")) //If player attempts to jump
        {
            if (grounded) //jump normally
            {
                velocity.y = jumpTakeOffSpeed;
            }

            else if (canWallJump) //reverse direction and jump
            {
                velocity.y = jumpTakeOffSpeed;
                move.x *= -1;
                timer = Time.time + wallJumpTimer;
            }
        }
        else if (Input.GetButtonUp("Jump")) //If jump is no longer pressed
        {
            if (velocity.y > 0) //Vertical velocity will begin to decrease
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        ///////////////////////// ANIMATION ////////////////////////////////////
        //bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        /*if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);*/
        ////////////////////////// END OF ANIMATION ///////////////////////////////

        targetVelocity = move * maxSpeed;
    }

    protected override void WallJump(int index)
    {
        if (hitBufferList[index].collider.gameObject.tag == "Wall")
        {
            //Debug.Log("HIT WALL!");
            canWallJump = true;
        }
    }

    /*protected void FixedUpdate() /////////////////////////    Does this work well with the parent FixedUpdate method? ////////////////////////////////
    {

    }*/

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            canWallJump = true;
        else
            canWallJump = false;
            
    }
}


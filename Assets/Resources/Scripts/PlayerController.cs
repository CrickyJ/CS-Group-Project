using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FROM: https://unity3d.com/learn/tutorials/topics/2d-game-creation/player-controller-script?playlist=17093

public class PlayerController : PhysicsObject
{
    //Movement:
    public float maxSpeed = 7, jumpTakeOffSpeed = 7;
    //public float jumpTakeOffSpeed = 7;

    //Wall Jumping:
    [Tooltip ("Horizontal input delay (in seconds) after wall-jumping.")]
    [SerializeField] float wallJumpTimer = 0.5f; //Time after walljump to block input
    private float timer = 0.0f; //tracks elapsed time
    private bool jumpedLeft; //tracks player direction just after walljump

    //Animation:
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    bool flipSprite = false;

    //Shooting:
    [SerializeField] float fireRate; //Time required to pass between shots
    [SerializeField] GameObject projectile; //Object / Prefab spawned for projectile
    [SerializeField] Transform shotSpawn; //Position shot will spawn from

    void Awake() //Initialize Player Object
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity() //Called every frame by base class: PhysicsObject
    {
        animator.SetBool("isSliding", canWallJump);

        Vector2 move = Vector2.zero; //Reset movement vector for input & calculations

        move.x = Input.GetAxis("Horizontal");

        if ((move.x > 0) == jumpedLeft && Time.time < timer) //If player is still holding right after jumping from wall on their right
        {
            //Debug.Log("INPUT REVERSED");
            move.x *= -1; //Reverse input
        }

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
                if (move.x > 0) //if player was holding right while walljumping
                    jumpedLeft = true; //they want to jump left
                else //if player was holding left
                    jumpedLeft = false; //they want to jump right
                move.x *= -1;
                timer = Time.time + wallJumpTimer; //Start timer
                //Debug.Log("Starting timer.");
            }
        }
        else if (Input.GetButtonUp("Jump")) //If jump is no longer pressed
        {
            if (velocity.y > 0) //Vertical velocity will begin to decrease
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        animateSprite(move);

        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("FIRE");
            FireWeapon();
        }

        

        targetVelocity = move * maxSpeed;
    }

    private void animateSprite(Vector2 dir) //Determines direction for player sprite
    {
        flipSprite = (spriteRenderer.flipX ? (dir.x > 0.01f) : (dir.x < -0.01f));
        if (flipSprite)
        {
            
            spriteRenderer.flipX = !spriteRenderer.flipX;
            shotSpawn.localPosition = new Vector3(-shotSpawn.localPosition.x, shotSpawn.localPosition.y, shotSpawn.localPosition.z);
            //shotSpawn.Rotate(Vector3.forward * 180);
            shotSpawn.localRotation *= Quaternion.Euler(0, 0, 180); //Flip shotspawn
            //Debug.Log("FLIP: " + shotSpawn.rotation);
        }

        //animator.SetBool("grounded", grounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
    }

    private void FireWeapon() //Determines how player will fire weapon
    {
        //GameObject shot = GameObject.Instantiate(projectile, shotSpawn); //Instantiate shot at shotSpawn position and rotation
        Instantiate(projectile, shotSpawn.position, shotSpawn.rotation); //spawn shot -- movement handled by shotController
        //GetComponent<AudioSource>().Play(); //play audio attached to shot object
    }

    protected override void WallSlide(int index) //If player is colliding with wall
    {
        //if (hitBufferList[index].collider.gameObject.tag == "Environment")
        if (hitBufferList[index].collider.gameObject.CompareTag("Environment"))
        {
            //Debug.Log("HIT WALL @ Velocity.y = " + velocity.y);
            canWallJump = true;
        }
    }

    /*private void OnCollissionStay2D(Collision2D collision)
    {
        if (grounded) return; //Cannot wall jump if on ground

        if (collision.gameObject.tag == "Wall")
            canWallJump = true;
        else
            canWallJump = false;
            
    }*/
}


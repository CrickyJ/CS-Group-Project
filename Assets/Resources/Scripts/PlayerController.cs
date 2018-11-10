using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FROM: https://unity3d.com/learn/tutorials/topics/2d-game-creation/player-controller-script?playlist=17093

public class PlayerController : PhysicsObject
{
    
    #region Movement
    [Header("Movement")]
    //Basic Movement:
    [SerializeField] private bool enableMoreJumps = true;
    public float maxSpeed = 7;          //Horizontal speed
    Vector2 move; //Changes based on input
    public float jumpTakeOffSpeed = 7;  //Jump height
    [SerializeField] private int jumpsAllowed = 2;
    private int jumpNumber = 0;
    #endregion

    #region Dashing
    [Header("Dashing")]
    [SerializeField] private bool enableDash = true;
    [SerializeField] float dashSpeed = 5;         //Speed of dash
    [SerializeField] float dashTime = 0.5f;       //Time length of dash
    private float nextDash = 0.0f;      //Cooldown for dash
    private bool dashing = false;       //True if player presses dash button
    #endregion

    #region Wall Jumping
    [Header("Wall Jump")]
    [SerializeField] private bool enableWallJump = true;
    [Tooltip ("Horizontal input reversed right after wall jumping")]
    [SerializeField] float wallJumpTimer = 0.5f; //Time after walljump to block input
    private float timer = 0.0f; //tracks elapsed time
    private bool jumpedLeft; //tracks player direction just after walljump
    #endregion

    #region Animation
    //Animation:
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    bool flipSprite = false;
    #endregion

    #region Shooting
    //Shooting:
    [Header("Shooting")]
    [SerializeField] float shotCooldown = 0.2f; //Time required to pass between shots
    private float nextShot = 0.0f;
    [SerializeField] GameObject projectile; //Object / Prefab spawned for projectile

    [SerializeField] Transform shotSpawn; //Position shot will spawn from
    [SerializeField] float crouchShot; //How much crosshair is lowered while crouching
    private float shotSpawnDist; //default position of shotSpawn
    private float shotSpawnDiag; //used to calculate position for diagonal shooting
    enum facing {right=1, upright, up, upleft, left, downleft, down, downright} //possible aiming directions
    facing direction=facing.right; //current aiming direction
    bool crouching = false;//crouched or standing
    #endregion

    #region Status
    //Status:
    [Header("Status")]
    [SerializeField] int maxHP = 100;
    int health;
    [Tooltip ("Invincibility time (in seconds) after being hurt")]
    [SerializeField] float recoverTime = 0.5f; //Recovery time in seconds
    private IEnumerator coroutine;
    private bool invincible=false; //Player cannot be hurt while invincible
    #endregion

    void Awake() //Initialize Player Object
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        shotSpawnDist = shotSpawn.localPosition.x;
        shotSpawnDiag = shotSpawnDist / 1.4142f; //diagonal aiming x-y coordinates are 1/sqrt(2) of shotSpawnDist
        health = maxHP;
        if (!enableMoreJumps) jumpsAllowed = 1; //Only one jump if multiple is not allowed
    }

    protected override void ComputeVelocity() //Called every frame by base class: PhysicsObject
    {
        animator.SetBool("isSliding", canWallJump);
        move = Vector2.zero; //Reset movement vector for input & calculations
        crouching = false;
        move.x = Input.GetAxisRaw("Horizontal");
        if (grounded) jumpNumber = 0;

        if ((move.x > 0) == jumpedLeft && Time.time < timer) //If player is moving towards wall after jumping
        {
            move.x *= -1; //Reverse input
        }

        if (Input.GetButtonDown("Jump")) //When jump is first pressed
        {
            StartJump(); //Move player upward
        }
        else if (Input.GetButtonUp("Jump")) //When no longer holding jump
        {
            StopJump(); //Stop upward force
        }

        float vert = Input.GetAxisRaw("Vertical"); //input for this frame is stored
        Aim(vert);

        if (Input.GetButtonDown("Dash") && Time.time > (nextDash + dashTime*2)) //Speed player up
        {
            StartDash();
        }
        ContinueDash(); //Speeds up player for a brief time after pressing dash       

        if (Input.GetButton("Fire1")) //If holding fire button
        {
            FireWeapon();
        }

        targetVelocity = move * maxSpeed; //Set velocity to be computed in PhysicsObject script
    }

    private void StartJump()
    {
        
        if (grounded) //jump normally
        {
            velocity.y = jumpTakeOffSpeed;
            jumpNumber++;
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
        }
        else if (jumpNumber < jumpsAllowed) //double
        {
            velocity.y = jumpTakeOffSpeed;
            jumpNumber++;
        }
    }

    private void StopJump()
    {
        if (velocity.y > 0) //Vertical velocity will begin to decrease
        {
            velocity.y = velocity.y * 0.5f;
        }
    }

    private void Aim(float vert)
    {
        if (vert > 0) //If up button is held
        {
            if (move.x > 0.01) direction = facing.upright;
            else if (move.x < -0.01) direction = facing.upleft;
            else direction = facing.up;
        }

        else if (vert < 0) //If down button is held
        {
            if (grounded)
            {
                crouching = true;
            }
            //if (move.x > 0.01) direction = facing.downright;
            //else if (move.x < -0.01) direction = facing.downleft;
            else
            {
                direction = facing.down;
                crouching = false;
            }
        }

        else if (Input.GetAxisRaw("Aim") > 0) //Aim diagonally up
        {
            switch (direction)
            {
                case facing.right:
                    direction = facing.upright;
                    break;
                case facing.left:
                    direction = facing.upleft;
                    break;
                default:
                    break;
            }
        }

        else if (Input.GetAxisRaw("Aim") < 0) //Aim diagonally down
        {
            switch (direction)
            {
                case facing.right:
                    direction = facing.downright;
                    break;
                case facing.left:
                    direction = facing.downleft;
                    break;
                default:
                    break;
            }
        }

        else if (move.x > 0) //If moving right
        {
            direction = facing.right;
        }
        else if (move.x < 0) //If moving left
        {
            direction = facing.left;
        }
        else //if standing still, default to left or right direction
        {
            switch (direction)
            {
                case facing.upright:
                case facing.downright:
                    direction = facing.right;
                    break;
                case facing.upleft:
                case facing.downleft:
                    direction = facing.left;
                    break;
                case facing.up:
                case facing.down:
                    if (flipSprite) direction = facing.left;
                    else direction = facing.right;
                    break;
                default:
                    break;
            }
        }

        Animate(direction); //animate sprite and move crosshair
    }

    private void Animate(facing dir) //changes sprite and shotspawn location
    {
        float x = shotSpawnDist, y = 0, rot=0;
        switch(dir)
        {
            case facing.right: //Reset to default
                flipSprite= false;
                //shotSpawn.localPosition = new Vector3(shotSpawnDist, 0, 0);
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, 0);
                animator.SetInteger("direction", 0); //resets animation to default
                break;

            case facing.upright:
                flipSprite = false;
                //shotSpawn.localPosition = new Vector3(shotSpawnDiag, shotSpawnDiag, 0);
                x = shotSpawnDiag; y = shotSpawnDiag;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, 45);
                rot = 45;
                animator.SetInteger("direction", 2);
                break;

            case facing.up:
                //shotSpawn.localPosition = new Vector3(0, shotSpawnDist, 0);
                x = 0; y = shotSpawnDist;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, 90);
                rot = 90;
                animator.SetInteger("direction", 1);
                break;

            case facing.upleft:
                flipSprite = true;
                //shotSpawn.localPosition = new Vector3(-shotSpawnDiag, shotSpawnDiag, 0);
                x = -shotSpawnDiag; y = shotSpawnDiag;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, 135);
                rot = 135;
                animator.SetInteger("direction", 2);
                break;

            case facing.left:
                flipSprite = true;
                //shotSpawn.localPosition = new Vector3(-shotSpawnDist, 0, 0);
                x = -shotSpawnDist; //y = 0;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, 180);
                rot = 180;
                animator.SetInteger("direction", 0); //resets animation to default (but flipped sprite)
                break;

            case facing.downleft:
                flipSprite = true;
                //shotSpawn.localPosition = new Vector3(-shotSpawnDiag, -shotSpawnDiag, 0);
                x = -shotSpawnDiag; y = -shotSpawnDiag;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, -135);
                rot = -135;
                animator.SetInteger("direction", 3);
                break;

            case facing.down:
                //shotSpawn.localPosition = new Vector3(0, -shotSpawnDist, 0);
                x = 0; y = -shotSpawnDist;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, -90);
                rot = -90;
                animator.SetInteger("direction", 4);
                break;

            case facing.downright:
                flipSprite = false;
                //shotSpawn.localPosition = new Vector3(shotSpawnDiag, -shotSpawnDiag, 0);
                x = shotSpawnDiag; y = -shotSpawnDiag;
                //shotSpawn.localRotation = Quaternion.Euler(0, 0, -45);
                rot = -45;
                animator.SetInteger("direction", 3);
                break;

            default:
                Debug.Log("DEFAULT");
                break;
        }
        spriteRenderer.flipX = flipSprite; //Flip sprite if facing left
        if (crouching) y -= crouchShot;
        shotSpawn.localPosition = new Vector2(x, y); //Move crosshair for weapon
        shotSpawn.localRotation = Quaternion.Euler(0, 0, rot);
    }

    private void FireWeapon() //Determines how player will fire weapon
    {
        if (Time.time > nextShot)
        {
            Instantiate(projectile, shotSpawn.position, shotSpawn.rotation); //spawn shot -- movement handled by shotController
            //GetComponent<AudioSource>().Play(); //play audio attached to shot object
            nextShot = Time.time + shotCooldown;
        }
    }

    private string printHealth()
    {
        return "HP = " + health + " / " + maxHP;
    }

    private void StartDash()
    {
        //Debug.Log("Start Dash");
        dashing = true; //Player speed will be increased
        nextDash = Time.time + dashTime; //Sets time when player can next dash
    }

    private void ContinueDash()
    {
        if (dashing && Time.time < nextDash) //Continue dash
        {
            move.x *= dashSpeed;
            velocity.y = 0;
            //Debug.Log("Still dashing");
        }
        else dashing = false; //End dash after period of time
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

    private void OnTriggerEnter2D(Collider2D collision) //For the first frame a player touches collider
    {
        if(collision.gameObject.CompareTag("PickUp")) //Gathering pickups
        {
            if (health < maxHP) //If health is not at maximum
            {
                health += 10;
                if (health > maxHP) health = maxHP; //set health to max if it goes over
                Debug.Log(printHealth());
            }
            else
            {
                Debug.Log("Health Full!" + printHealth());
                return;
            }
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    //private void OnCollsionEnter2d(Collision2D collision)
    {
        if (!invincible && collision.gameObject.CompareTag("Enemy")) //Hitting Enemy
        {
            health -= 10;
            knockBack();
            //Invincible for time
            //Yield wait for seconds -- coroutine starts and waits a period of time
            //No longer invincible
        }

        else if (!invincible && collision.gameObject.CompareTag("Hazard")) //Hitting hazard or enemy projectile
        {
            health -= 10;
            knockBack();
            //Yield wait for seconds -- coroutine starts and waits a period of time
        }
        //else if (invincible) Debug.Log("Damage Blocked");
    }

    private void knockBack()
    {
        coroutine = IsInvincible();
        StartCoroutine(coroutine);
        velocity.y = jumpTakeOffSpeed / 2;
        if(!flipSprite)
        {
            targetVelocity.x = -10; //TODO: coroutine here?
        }
        else
        {
            targetVelocity.x = 10;
        }
        Debug.Log(printHealth());
    }

    private IEnumerator IsInvincible() //Player is invincible for a short amount of time
    {
        Debug.Log("Now invincible");
        invincible = true;
        yield return new WaitForSeconds(recoverTime);
        invincible = false;
        Debug.Log("NOT Invincible");
    }

    private IEnumerator Dashing() //Player continues to dash
    {
        dashing = true;
        yield return new WaitForSeconds(dashTime);
        dashing = false;
    }

    private IEnumerator Sliding() //Player does not move away from wall for a set time
    {
        //sliding = true;
        yield return new WaitForSeconds(wallJumpTimer);
        //sliding = false;
    }
}
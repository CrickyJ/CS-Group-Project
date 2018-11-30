using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FROM: https://unity3d.com/learn/tutorials/topics/2d-game-creation/player-controller-script?playlist=17093
//NOTE: Objects using this script are intended to be kinematic

public class PhysicsObject : MonoBehaviour
{
    [Header("Physics Attributes")]
    [Tooltip ("How flat ground must be for player to be still, default is 0.65")]
    public float minGroundNormalY = .65f; //How flat ground must be for player to be still
    [SerializeField] protected float gravityModifier = 1f; //Default gravity modifier

    [Tooltip ("How much player is slowed while contacting wall, default is 1")]
    [SerializeField] protected float wallFriction = 1f; //gravity modifier while sliding on wall

    protected Vector2 targetVelocity; //Protected can be accessed by child classes
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16]; //Raycast Array checks certain direction to see what is nearby
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);//Raycast List only keeps track of current collisions

    protected bool canWallJump;


    protected const float minMoveDistance = 0.001f; //Minimum distance to move in order to check collision (will not check if stopped)
    protected const float shellRadius = 0.01f; //Checks for colliders nearby, making sure player does not get stuck

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); //Layer masks determines what to ignore (check Physics2D layer collision matrix)
        contactFilter.useLayerMask = true;
    }

    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() //Called in PlayerController class
    {

    }

    protected virtual void WallSlide(int index) { } //Called in PlayerController if contacting wall
    protected virtual void CheckHit(int index) { }

    void FixedUpdate()
    {
        if (canWallJump && velocity.y < 0) //if player is sliding down wall
        {
                velocity += gravityModifier / wallFriction * Physics2D.gravity * Time.deltaTime; //calculate using wallFriction
                //Debug.Log("Slowing fall...");
        }
        else
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime; //calculate using default gravity
        velocity.x = targetVelocity.x;

        grounded = false;
        canWallJump = false; //player walljump status is reset before movement calculation

        Vector2 deltaPosition = velocity * Time.deltaTime; //calculate position change

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false); //set horizontal movement

        move = Vector2.up * deltaPosition.y; //calculate new vertical position

        Movement(move, true); //set vertical movement
    }

    void Movement(Vector2 move, bool yMovement) //Sets position of rigidbody2D
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance) //Only true if character is attempting to move
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear(); //clear list of previous collisions
            for (int i = 0; i < count; i++) //Insert current collisions into list
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) //Normal is calculated to determine angle of object
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY) //if angle works as "ground"
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                else if (!grounded) //If player is NOT grounded
                {
                    WallSlide(i); //If collider is NOT ground, attempt to WallJump
                    CheckHit(i);
                    //Debug.Log("Trying to move: " + distance);
                }

                float projection = Vector2.Dot(velocity, currentNormal); //Prevents player from getting stuck in colliders (and zeroing velocity)
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

}
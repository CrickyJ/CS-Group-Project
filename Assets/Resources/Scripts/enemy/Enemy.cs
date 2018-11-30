 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class Enemy : MonoBehaviour {
    //Components
    public Animator animator;
    protected Rigidbody2D rb;
    //private EnemyStateHandler stateHandler;
    [HideInInspector] public BoxCollider2D col;
    public GameObject EdgeTriggerFolder;
    public EnemyEdgeChecker[] EdgeTriggers;

    //Health
    [HideInInspector] public float Health;
    public float HealthMax;

    //Movement
    /// <summary>
    /// How far from the center of this object the bottom of their collider is.
    /// </summary>
    public float GroundDistance;
    public bool Flying = false; //sets movetype
    protected enum MOVE_TYPE {
        WALKING, //Changes to DesiredVelocity will not modify Y-Speed
        FLYING //Changes to DesiredVelocity will modify Y-Speed
    };
    protected MOVE_TYPE MoveType;
    public Vector2 DesiredVelocity; 
    

	// Unity functions
    void Awake () {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

	public virtual void Start () {
        Health = HealthMax;
        //stateHandler = GetComponent<EnemyStateHandler>();

        //set appropriate move type
        if(Flying) {
            MoveType = MOVE_TYPE.FLYING;
        }
        else {
            MoveType = MOVE_TYPE.WALKING;
        }
	}

    public virtual void Update () {
        DoMovement();

    }

    /*
    ContactPoint2D[] contactPoints = new ContactPoint2D[8];
    public virtual void FixedUpdate() {
        //try and get over erroneous corner collisions
        ContactFilter2D filter = new ContactFilter2D();
        filter.maxNormalAngle = 1;
        filter.useNormalAngle = true;
        int numpoints = col.GetContacts(filter, contactPoints);
        Debug.Log(numpoints);
    }
    */

    // Damage / Death
    /// <summary>
    /// Attempt to apply damage to this enemy
    /// </summary>
    /// <param name="amount">the amount of damage to apply</param>
    public void ApplyDamage( float amount ) {
        float newHealth = Health - amount;

        if (newHealth <= 0 ) {
            DoDeath();
        }
        else {
            Health = newHealth;
            animator.SetTrigger("OnHurt");
        }
    }
    protected void DoDeath() {
        animator.SetTrigger("OnDeath");
    }
    public void DoPostDeath() {
        Destroy(this.gameObject);
    }

    // Movement
    protected void DoMovement() {
        //update rigidbody velocity to reflect DesiredVelocity
        if(MoveType == MOVE_TYPE.FLYING) {
            rb.velocity = DesiredVelocity;
        }
        else {
            rb.velocity = new Vector2(DesiredVelocity.x,rb.velocity.y);
        }
    }

    /// <summary>
    /// Get all colliders touching the enemy in the given direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="layermask"></param>
    /// <returns></returns>
    public int GetObstructions(Direction direction, ContactFilter2D filter, Collider2D[] results) {
        return EdgeTriggers[(int)direction].GetOverlaps(filter, results);
    }
    /// <summary>
    /// check if the enemy is on the ground
    /// </summary>
    /// <returns></returns>

    Collider2D[] results = new Collider2D[4];
    public bool IsGrounded() {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        int numResults = GetObstructions(Direction.Down, filter, results);
        for(int i = 0; i < numResults; i++) {
            if (results[i].CompareTag("Environment")) return true;
        }
        return false;
    }
    /// <summary>
    /// check if the CENTER of the player is above the ground
    /// </summary>
    /// <returns></returns>
    public bool IsGroundBelow() {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        
        //EdgeTriggers[4] should be the special immediately below box
        int numResults = EdgeTriggers[4].GetOverlaps(filter, results);
        for(int i = 0; i < numResults; i++) {
            if (results[i].CompareTag("Environment")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// creates Bounds objects along each edge of the enemy's collision box.
    /// should call this again if you change their collision box.
    /// </summary>
    public Bounds[] GenerateObstructionShapes() {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        Bounds[] obstructionBounds = new Bounds[5];
        float bufferThickness = 0.01f;
        float sizeFraction = 0.9f;
        float edgeSize = 0.1f;

        //this will break if I ever mess too heavily with the indices of the Direction enum
        //which is kinda bad practice. A map would resolve this problem but would be slower

        //right
        obstructionBounds[(int)Direction.Right] = new Bounds(
            new Vector2(col.size.x / 2 + bufferThickness / 2, 0) + col.offset,
            new Vector2(edgeSize, col.size.y * sizeFraction)
        );

        //left
        obstructionBounds[(int)Direction.Left] = new Bounds(
            new Vector2(-col.size.x / 2 - bufferThickness / 2, 0) + col.offset,
            new Vector2(edgeSize, col.size.y * sizeFraction)
        );

        //up
        obstructionBounds[(int)Direction.Up] = new Bounds(
            new Vector2(0, col.size.y / 2 + bufferThickness / 2) + col.offset,
            new Vector2(col.size.x * sizeFraction, edgeSize)
        );

        //down
        obstructionBounds[(int)Direction.Down] = new Bounds(
            new Vector2(0, -col.size.y / 2 - bufferThickness / 2) + col.offset,
            new Vector2(col.size.x * sizeFraction, edgeSize)
        );

        //special "below" boundary
        obstructionBounds[4] = new Bounds(
            new Vector2(0, -col.size.y / 2 - bufferThickness / 2) + col.offset,
            new Vector2(edgeSize, edgeSize)
        );

        return obstructionBounds;
    }
    

}

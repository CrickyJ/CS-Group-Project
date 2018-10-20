using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class Enemy : MonoBehaviour {
    //Components
    private Animator Animator;
    protected Rigidbody2D rb;

    //Health
    float Health;
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
    [HideInInspector] public Vector2 DesiredVelocity; 
    

	// Unity functions
	public virtual void Start () {
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Health = HealthMax;

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
    private void OnValidate() {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null ) {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

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
            Animator.SetTrigger("OnHurt");
        }
    }
    protected void DoDeath() {
        Animator.SetTrigger("OnDeath");
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior : MonoBehaviour {
    public abstract void PerformAction();

    
    //saving some components here for convenience functions
    protected Enemy enemy;
    protected BoxCollider2D col;

    public virtual void Start() {
        enemy = GetComponent<Enemy>();
        col = GetComponent<BoxCollider2D>();
    }

    // useful functions for behaviors
    public bool IsGrounded() {
        //do an overlapCircle near our feet to see if there's ground there
        return CheckDirection(Vector2.down);
    }
    public bool CheckDirection(Vector2 direction) {

        //if overlapCircle returns anything BUT null then we've found something
        return null != Physics2D.OverlapCircle(transform.position + (Vector3)(direction * col.size / 2), 0.1f, LAYER.SOLIDS);
    }

}

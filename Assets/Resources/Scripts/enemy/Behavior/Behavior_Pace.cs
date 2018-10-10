using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Pace : Behavior {

    public float WalkSpeed = 5;
    /// <summary>
    /// if true, the enemy will turn when it reaches a ledge. if false, it will fall off the ledge.
    /// </summary>
    public bool DoTurnAtLedges = true;
    /// <summary>
    /// Time in seconds the enemy will be still when changing direction
    /// </summary>
    public float TurnPauseTime = 0.25f;

    bool isPaused = false;
    float nextTurnTime;

    public override void PerformAction() {
        if(isPaused) {
            //if we're supposed to wait to turn
            if (Time.time > nextTurnTime)
                //if we're done waiting turn
                ResolveChangeDirection();
            else {
                //keep waiting
                enemy.DesiredVelocity = Vector2.zero;
                return;
            }
        }

        if (CheckDirection(enemy.FacingRight ? Vector2.right : Vector2.left)) {
            //if there's a wall in front of us in the direction we're going
            //change directions
            ChangeDirection();
        }
        else if (DoTurnAtLedges && !CheckDirection(enemy.FacingRight ? new Vector2(0.5f,-1) : new Vector2(-0.5f, -1))) {
            //if we're supposed to turn at a ledge, and there isn't anything at our feet right in front of us
            //change directions
            ChangeDirection();
        }

        enemy.DesiredVelocity = new Vector2(WalkSpeed * (enemy.FacingRight ? 1 : -1), 0);
    }
    void ChangeDirection() {
        if(TurnPauseTime > 0) {
            //we're supposed to wait for TurnPauseTime before turning around, so wait
            isPaused = true;
            nextTurnTime = Time.time + TurnPauseTime;
        }
        else {
            //just instantly change direction
            ResolveChangeDirection();
        }
    }
    void ResolveChangeDirection() {
        isPaused = false;
        enemy.FacingRight = !enemy.FacingRight;
    }
}

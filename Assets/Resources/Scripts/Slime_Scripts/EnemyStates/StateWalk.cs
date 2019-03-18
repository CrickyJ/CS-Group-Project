using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWalk : EnemyState {
    readonly bool triggerAtWall = true;
    readonly bool triggerAtLedge = true;
    readonly bool triggerAtAlly = true;

    readonly ContactFilter2D filter;
    
    public Direction direction = Direction.Right;
    public float speed = 1;

    bool ledgeTriggerCooldown = false;
    readonly float ledgeCooldownTime = 0.3f;
    
    public StateWalk(EnemyStateHandler _stateHandler, Enemy _enemy, Direction initialDirection, 
        bool _triggerAtWall, bool _triggerAtLedge, bool _triggerAtAlly, float initialSpeed) 
        : base(_stateHandler, _enemy) {
        direction = initialDirection;
        triggerAtWall = _triggerAtWall;
        triggerAtLedge = _triggerAtLedge;
        triggerAtAlly = _triggerAtAlly;
        speed = initialSpeed;

        filter = new ContactFilter2D();
        filter.useTriggers = false;
    }

    public void FlipDirection() {
        if(direction == Direction.Right) {
            direction = Direction.Left;
        }
        else {
            direction = Direction.Right;
        }
    }



    public override bool Perform() {
        int xDir = direction == Direction.Right ? 1 : -1;
        enemy.DesiredVelocity = new Vector2(xDir * speed, 0);

        if(triggerAtWall || triggerAtAlly) {
            //check for things in the direction we are moving
            //if we hit a thing we should trigger from, then trigger
            Collider2D[] results = new Collider2D[4];
            int numResults = enemy.GetObstructions(direction,filter, results);
            
            for(int i = 0; i < numResults; i++) {
                //ignore hitting ourselves
                if (results[i] == enemy.col)
                    continue;

                string tag = results[i].tag;

                if(triggerAtWall && tag == "Environment") {
                    return true;
                }
                else if(triggerAtAlly && tag == "Enemy") {
                    return true;
                }
            }
        }
        if(ledgeTriggerCooldown && EnterStateTime + ledgeCooldownTime < Time.time) {
            ledgeTriggerCooldown = false;
        }
        
        if(triggerAtLedge && !ledgeTriggerCooldown && enemy.IsGrounded() && !enemy.IsGroundBelow()) {
            ledgeTriggerCooldown = true;
            return true;
        }

        return base.Perform();
    }
}

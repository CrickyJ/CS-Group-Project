using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWait : EnemyState {

    private float triggerTime;
    private float duration;

    public StateWait(EnemyStateHandler _stateHandler, Enemy _enemy, float _duration)
        : base(_stateHandler, _enemy) {
        duration = _duration;
    }

    public override void OnEnterState() {
        ResetTriggerTime();
        base.OnEnterState();
    }

    public void SetDuration(float newDuration) {
        float diff = newDuration - duration;
        triggerTime += diff;
        duration = newDuration;
    }
    public void ResetTriggerTime() {
        if(duration < 0) {
            triggerTime = -1;
        }
        else {
            triggerTime = Time.time + duration;
        }
    }

    public override bool Perform() {
        enemy.DesiredVelocity = Vector2.zero;
        if( triggerTime != -1 && Time.time > triggerTime ) {
            return true;
        }

        return base.Perform();
    }
}

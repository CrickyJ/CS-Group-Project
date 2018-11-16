using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWaitDamage : StateWait {

    private float lastHP;

    public StateWaitDamage(EnemyStateHandler _stateHandler, Enemy _enemy, float _fallbackDuration)
        : base(_stateHandler, _enemy, _fallbackDuration) {
    }

    public override void OnEnterState() {
        lastHP = enemy.Health;
        base.OnEnterState();
    }

    public override bool Perform() {
        if( enemy.Health < lastHP) {
            return true;
        }
        else {
            lastHP = enemy.Health;
        }

        return base.Perform();
    }
}

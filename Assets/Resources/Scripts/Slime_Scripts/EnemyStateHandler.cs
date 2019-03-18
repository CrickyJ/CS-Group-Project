using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateHandler : MonoBehaviour {

    //components
    protected Enemy enemy;

    private EnemyState activeState;
    public EnemyState ActiveState {
        get {
            return activeState;
        }
        set {
            if(activeState != null) activeState.OnExitState();
            value.OnEnterState();
            activeState = value;
        }
    }
    
    public virtual void Start() {
        enemy = GetComponent<Enemy>();
    }

    public bool PerformActiveState() {
        return activeState.Perform();
    }
}

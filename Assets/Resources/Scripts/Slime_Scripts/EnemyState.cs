using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState {

    protected bool IsActive = false;
    protected readonly Enemy enemy;
    protected readonly EnemyStateHandler stateHandler;
    float enterStateTime;
    public float EnterStateTime {
        get {
            return enterStateTime;
        }
    }

    public EnemyState(EnemyStateHandler _stateHandler, Enemy _enemy) {
        enemy = _enemy;
        stateHandler = _stateHandler;
        Start();
    }

    public virtual void Start() {
        
    }

    public virtual void OnEnterState() {
        enterStateTime = Time.time;
        IsActive = true;
    }
    public virtual void OnExitState() {
        IsActive = false;
    }
    /// <summary>
    /// perform this state's action
    /// </summary>
    /// <returns>true if a triggered event occured, else false</returns>
    public virtual bool Perform() {
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandlerSlime : EnemyStateHandler {

    public Direction initialDirection;
    public float Speed;
    public float TurnDuration;
    public bool isWaiting;

    private StateWalk WalkingState;
    private StateWait TurningState;

    private Direction faceDirection;
    public Direction FaceDirection {
        get { return faceDirection; }
        set {
            enemy.animator.SetBool("FaceRight", (value == Direction.Right) ? true : false);
            faceDirection = value;
        }
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        WalkingState = new StateWalk(this, enemy, initialDirection, true, true, true, Speed);
        TurningState = new StateWait(this, enemy, TurnDuration);

        ActiveState = WalkingState;
        FaceDirection = WalkingState.direction;
    }
	
	// Update is called once per frame
	void Update () {
        bool didTrigger = PerformActiveState();
        isWaiting = (ActiveState == TurningState);

        if ( didTrigger ) {
            if (ActiveState == WalkingState) {
                WalkingState.FlipDirection();
                ActiveState = TurningState;
            }
            else {
                ActiveState = WalkingState;
                FaceDirection = WalkingState.direction;
            }
        }
	}

    private void OnValidate() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(initialDirection == Direction.Right) {
            spriteRenderer.flipX = true;
        }
        else {
            spriteRenderer.flipX = false;
        }
    }
}

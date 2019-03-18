using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandlerBigSlime : EnemyStateHandler {

    public Direction initialDirection = Direction.Right;
    public float minClosedWaitTime = 0.5f;
    public float maxClosedWaitTime = 2f;
    public float maxOpenWaitTime = 1f;
    public ShotSpawner[] ShotSpawners;

    private StateWait WaitEyeClosed;
    private StateWaitDamage WaitEyeOpen;

    // Use this for initialization
    public override void Start() {
        base.Start();
        enemy.animator.SetBool("FaceRight", (initialDirection == Direction.Right) ? true : false);
        enemy.animator.SetBool("EyeClosed", true);

        WaitEyeClosed = new StateWait(this, enemy, GetRandomClosedWaitTime());
        WaitEyeOpen = new StateWaitDamage(this, enemy, maxOpenWaitTime);
        ActiveState = WaitEyeClosed;
	}
	
	// Update is called once per frame
	void Update () {
        bool didTrigger = PerformActiveState();
        
        if(didTrigger) {
            if(ActiveState == WaitEyeClosed) {
                ActiveState = WaitEyeOpen;
                enemy.animator.SetBool("EyeClosed", false);
            }
            else { //if ActiveState == WaitEyeOpen
                //fire the shots, then start waiting to close the eye
                foreach(ShotSpawner spawner in ShotSpawners) {
                    spawner.SpawnProjectile((initialDirection == Direction.Right) ? true : false);
                }

                ActiveState = WaitEyeClosed;
                enemy.animator.SetBool("EyeClosed", true);
            }
        }
	}
    float GetRandomClosedWaitTime() {
        return Random.Range(minClosedWaitTime, maxClosedWaitTime);
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

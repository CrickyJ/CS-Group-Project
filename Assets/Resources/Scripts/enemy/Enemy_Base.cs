using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour {
    public float Health;
    public float HealthMax;
    float DamageInterval = 3;


    //Components
    private Animator Animator;
    

	// Use this for initialization
	void Start () {
        Animator = GetComponent<Animator>();
        Health = HealthMax;
	}

    void Update () {
    }
	
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

    public void DoDeath() {
        Animator.SetTrigger("OnDeath");
    }
}

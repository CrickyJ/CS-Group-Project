using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PhysicsObject {
    #region Shooting
    //Shooting:
    [Header("Shooting")]
    [SerializeField] float shotCooldown = 0.2f; //Time required to pass between shots
    [SerializeField] GameObject projectile; //Object / Prefab spawned for projectile
    [SerializeField] Transform shotSpawn; //Position shot will spawn from
    private float shotSpawnDist; //default position of shotSpawn
    bool isAlive = true;
    private IEnumerator shoot;
    #endregion

    // Use this for initialization
    void Start () {
        shoot = TryShoot();
        StartCoroutine(shoot);
	}

    private void Update() //Called every frame
    {
        //Face Player
        //Check collision
        //Die?
    }

    protected override void ComputeVelocity()
    {
    }

    private void FireWeapon() //Determines how player will fire weapon
    {
            Instantiate(projectile, shotSpawn.position, shotSpawn.rotation); //spawn shot -- movement handled by shotController
            //GetComponent<AudioSource>().Play(); //play audio attached to shot object
            //animator.SetTrigger("fire");
            //Debug.Log("Fire!");
    }

    private IEnumerator TryShoot()
    {
        while (isAlive)
        {
            FireWeapon();
            yield return new WaitForSeconds(shotCooldown);
        }
    }

   protected override void CheckHit(int index)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

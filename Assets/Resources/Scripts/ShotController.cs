using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {
    private Rigidbody2D rb;
    [SerializeField] float speed = 1, timeLimit = 2;

    ShotController(string dir) //Constructor
    {
        Debug.Log("dir");
    }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
       // rb.velocity = direction * speed;
        Destroy(this.gameObject, timeLimit); //Object will be destroyed after time limit
	}

    private void OnTriggerEnter2D(Collider2D hit)
    {
        //Debug.Log("HIT " + hit.gameObject.tag);
        if (hit.gameObject.CompareTag("Environment")) //If projectile collides with environement
        {
            //Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] Vector2 below;

    bool canJump = false;
    public Rigidbody2D rb;
    Vector2 curPos, dest;

    private SpriteRenderer player;
    private Animator anim;
    
	// Use this for initialization
	void Start () {
        dest = transform.position; //current position of object saved
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate() //Independent of frame rate
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); //Inputs set by Unity
        if (moveHorizontal == 0)
        {
            anim.SetBool("moving", false);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        { //Flip sprite based on movement direction
            player.flipX = false;
            anim.SetBool("moving", true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            player.flipX = true;
            anim.SetBool("moving", true);
        }
        //float moveVertical = Input.GetAxis("Vertical");
        //Vector2 movement = new Vector2(moveHorizontal, 0.0f);
        Vector2 movement = new Vector2(moveHorizontal * speed, 0.0f);
        //rb.velocity = movement * speed;
        //float jump = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.UpArrow) && canJump) //raycast if ground is below player, if TRUE allow jump
        {
            canJump = false;
            //jump *= 50;
            Vector2 up = new Vector2(0, 25 * jumpForce); //base upward force
            movement += up;
            //rb.AddForce(up * jumpForce, ForceMode2D.Impulse); //magnitude increased by player's jump power, added to object instantly by Impulse
            Debug.Log("JUMP: " + movement);
        }
        else
            grounded(below);
        rb.velocity = movement;
        //Debug.Log("Moving @" + movement);
    }

    // Update is called once per frame
    void Update () {
	}

    private void OnCollisionEnter2D(Collision2D collision) //Raycasting collision - test whether movement will collide player before actually moving them
    {
        //Debug.Log(collision.gameObject.name); //Print name of collision in console
        if (collision.gameObject.tag == "Hazard")
            transform.position = Vector2.zero;
    }

    void grounded(Vector2 dir) //Check if ground is present in player's downward direction
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos); //cast line from (pos + dir) to current (pos), check for colliders in line
        //Debug.Log(hit.collider.name);

        if (hit.collider.tag == "Ground")
            canJump = true; //only true if player is touching ground
        //return (hit.collider != GetComponent<Collider2D>());
    }
}

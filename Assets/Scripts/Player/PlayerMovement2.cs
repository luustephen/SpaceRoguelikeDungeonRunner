using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour {

    public float moveSpeed = 1f;
    public float runSpeed = 2f;
    public float speedModifier = 1.5f;
    private Rigidbody2D rb;
    private bool lockMovement = false;
    public GameObject node;

    Vector2 movement; 

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	void Update ()
    {
         movement.x = Input.GetAxisRaw("Horizontal");
         movement.y = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Node")
        {
            node = collision.gameObject;
        }
    }

    void FixedUpdate()
    {
        if (!lockMovement)
        {
            runSpeed = speedModifier * moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = movement * runSpeed;
            }
            else
            {
                rb.velocity = movement * moveSpeed;
            }
        }
    }

    public void LockMovement()
    {
        lockMovement = true;
    }

    public void UnlockMovement()
    {
        lockMovement = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour {

    public float moveSpeed = 6f;
    public float runSpeed;
    public float speedModifier = 1.5f;
    public Rigidbody2D rb;

    Vector2 movement; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical");

	}

    void FixedUpdate()
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

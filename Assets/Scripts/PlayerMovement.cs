using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public KeyCode up, down, left, right, jump;
    private float horizontalMovement, verticalMovement;
    private Rigidbody rb;
    public float speed;
    public float maxSpeed;

	// Use this for initialization
	void Start () {
        //TODO: Add some way to allow player to modify controls
        /*
        up = KeyCode.W;
        down = KeyCode.S;
        left = KeyCode.A;
        right = KeyCode.D;
        jump = KeyCode.Space;
        horizontalMovement = 0;
        verticalMovement = 0;
        turnSpeed = .25f;
        speed = .10f;*/
        horizontalMovement = 0;
        verticalMovement = 0;
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (Input.GetKey(up))                   //Accelerate to max speed
        {
            if(verticalMovement < maxSpeed)
                verticalMovement+= speed;
        }

        if (Input.GetKey(down))
        {
            if (verticalMovement > -maxSpeed)
                verticalMovement-= speed;
        }

        if (Input.GetKey(left))
        {
            if (horizontalMovement > -maxSpeed)
                horizontalMovement-= speed;
        }

        if (Input.GetKey(right))
        {
            if (horizontalMovement < maxSpeed)
                horizontalMovement +=speed;
        }

        if (Input.GetKey(jump))                         //Layer transition
        {

        }

        if(!Input.GetKey(left) && !Input.GetKey(right)) //Slow to standstill if not hitting any buttons
        {
            if (horizontalMovement < 0)
            {
                horizontalMovement += speed / 2;

                if (horizontalMovement > 0)
                    horizontalMovement = 0;     
            }
            else
            {
                horizontalMovement -= speed / 2;

                if (horizontalMovement <= 0)
                    horizontalMovement = 0;
            }
            
        }

        if (!Input.GetKey(up) && !Input.GetKey(down)) //Slow to standstill if not hitting any buttons
        {
            if (verticalMovement < 0)
            {
                verticalMovement += speed / 2;

                if (verticalMovement > 0)
                    verticalMovement = 0;
            }
            else
            {
                verticalMovement -= speed / 2;

                if (verticalMovement <= 0)
                    verticalMovement = 0;
            }
        }

        transform.Translate(horizontalMovement, 0,verticalMovement);
    }
}

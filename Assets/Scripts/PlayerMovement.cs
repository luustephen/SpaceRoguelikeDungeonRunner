﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public KeyCode up, down, left, right, jump;
    private float horizontalMovement, verticalMovement;
    private Rigidbody rb;
    public float speed;
    public float maxSpeed;
    private bool lockL, lockR, lockU, lockD;    //Is player not allowed to go that direction

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
        lockL = false;
        lockR = false;
        lockU = false;
        lockD = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (Input.GetKey(up) && !lockU)                   //Accelerate to max speed
        {
            if(verticalMovement <= maxSpeed)
                verticalMovement += speed;
        }

        if (Input.GetKey(down) && !lockD)
        {
            if (verticalMovement >= -maxSpeed)
                verticalMovement -= speed;
        }

        if (Input.GetKey(left) && !lockL)
        {
            if (horizontalMovement >= -maxSpeed)
                horizontalMovement -= speed;
        }

        if (Input.GetKey(right) && !lockR)
        {
            if (horizontalMovement <= maxSpeed)
                horizontalMovement +=speed;
        }

        if (Input.GetKey(jump))                         //Layer transition
        {

        }

        if(!Input.GetKey(left) && !Input.GetKey(right) || Input.GetKey(left) && Input.GetKey(right)) //Slow to standstill if not hitting any buttons or both
        {
            if (horizontalMovement < 0)
            {
                horizontalMovement += speed / 2;

                if (horizontalMovement >= 0)
                    horizontalMovement = 0;     
            }
            if(horizontalMovement > 0)
            {
                horizontalMovement -= speed / 2;

                if (horizontalMovement <= 0)
                    horizontalMovement = 0;
            }
            
        }

        if (!Input.GetKey(up) && !Input.GetKey(down) || Input.GetKey(up) && Input.GetKey(down)) //Slow to standstill if not hitting any buttons or both
        {
            if (verticalMovement < 0)
            {
                verticalMovement += speed / 2;

                if (verticalMovement >= 0)
                    verticalMovement = 0;
            }
            if (verticalMovement > 0)
            {
                verticalMovement -= speed / 2;

                if (verticalMovement <= 0)
                    verticalMovement = 0;
            }
        }

        if (lockU && verticalMovement > 0)
            verticalMovement = 0;
        if (lockD && verticalMovement < 0)
            verticalMovement = 0;
        if (lockR && verticalMovement > 0)
            horizontalMovement = 0;
        if (lockL && verticalMovement < 0)
            horizontalMovement = 0;

        print(horizontalMovement + " " + verticalMovement);
        transform.Translate(horizontalMovement, 0,verticalMovement);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Left Wall")        //Collide with walls
        {
            lockL = true;
            horizontalMovement = 0;
        }
        if (other.tag == "Right Wall")
        {
            lockR = true;
            horizontalMovement = 0;
        }
        if (other.tag == "Up Wall")
        {
            lockU = true;
            verticalMovement = 0;
        }
        if (other.tag == "Down Wall")
        {
            lockD = true;
            verticalMovement = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Left Wall")        //Release movement lock after you walk away from a wall
        {
            lockL = false;
        }
        if (other.tag == "Right Wall")
        {
            lockR = false;
        }
        if (other.tag == "Up Wall")
        {
            lockU = false;
        }
        if (other.tag == "Down Wall")
        {
            lockD = false;
        }
    }
}

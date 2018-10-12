using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public KeyCode forward, backward, left, right, jump;
    private float horizontalMovement, verticalMovement;
    public float speed;

	// Use this for initialization
	void Start () {
        //TODO: Add some way to allow player to modify controls
        /*
        forward = KeyCode.W;
        backward = KeyCode.S;
        left = KeyCode.A;
        right = KeyCode.D;
        jump = KeyCode.Space;
        horizontalMovement = 0;
        verticalMovement = 0;
        speed = .10f;*/
    }
	
	// Update is called once per frame
	void Update () {
        horizontalMovement = 0;
        verticalMovement = 0;
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        if (Input.GetKey(forward))
        {
            verticalMovement++;
        }

        if (Input.GetKey(backward))
        {
            verticalMovement--;
        }

        if (Input.GetKey(left))
        {
            horizontalMovement--;
        }

        if (Input.GetKey(right))
        {
            horizontalMovement++;
        }

        if (Input.GetKey(jump))                         //Layer transition
        {

        }

        transform.Translate(horizontalMovement * speed, 0,verticalMovement * speed);
        print(horizontalMovement+ " " + verticalMovement);
    }
}

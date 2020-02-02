﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    public float radiusOfLockOn = 5;
    public float forceOfLockOn = 10;
    private Rigidbody2D rb;
    private EnemySpawner enemySpawnScript;
    private bool isHoming = false;
    private int maxBounces = 0; //Number of times a projectile can hit a wall
    private int numBounces = 0;
    private GameObject previousObjectHit; //Previous object interacted with
    private GameObject player;
    private Vector2 previousVelocity;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>());
        rb = GetComponent<Rigidbody2D>();
        enemySpawnScript = FindObjectOfType<EnemySpawner>();
	}
	
	// Update is called once per frame
	void Update () {

        previousVelocity = rb.velocity;
        Vector2 velocity = rb.velocity;
        float angleOfVelocity;

        if(velocity.x >= 0) //Convert the velocity to the euler angle to be used for rotation calculations, there must be a better way of doing this
        {
            angleOfVelocity = Mathf.Rad2Deg * Mathf.Atan(velocity.y / velocity.x) + 90;
        }
        else
        {
            angleOfVelocity = Mathf.Rad2Deg * Mathf.Atan(velocity.y / velocity.x) + 270;
        }

        // print("velocity: " + velocity + " angle: " + angleOfVelocity);
        //print("rotation : " + transform.rotation.eulerAngles.z);
        //print(transform.rotation.eulerAngles.z - angleOfVelocity);

        if (isHoming)
        {
            GameObject[] allEnemies = enemySpawnScript.getEnemies();
            for (int i = 0; i < allEnemies.Length; i++) //Find nearest enemy and home onto it
            {
                if (allEnemies[i] != null && Mathf.Abs(allEnemies[i].transform.position.y - transform.position.y) < radiusOfLockOn && Mathf.Abs(allEnemies[i].transform.position.x - transform.position.x) < radiusOfLockOn)
                {
                    rb.AddForce((allEnemies[i].transform.position - transform.position) * forceOfLockOn);
                    transform.Rotate(new Vector3(0, 0, angleOfVelocity - transform.rotation.eulerAngles.z)); //Rotate the projectile to match the movement of any homing targets
                    continue;
                }
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node" && collision.gameObject.tag != "PickupItem")
        {
            if (numBounces >= maxBounces && previousObjectHit != collision.gameObject)
            {
                print(previousObjectHit);
                Destroy(gameObject);
            }

            numBounces++;
            //Vector2 previousVelocity = rb.velocity * rb.velocity.magnitude; //Store and stop the object's velocity
            rb.Sleep();

            ContactPoint2D contactPoint = collision.GetContact(0);
            Vector2 newVelocity = previousVelocity - (2 * Vector2.Dot(previousVelocity,contactPoint.normal) * contactPoint.normal); //Find reflection vector

            previousObjectHit = collision.gameObject;

            rb.AddForce(newVelocity * previousVelocity.magnitude); //Bounce off the wall at reflection angle
            float angle = Mathf.Atan2(newVelocity.y, newVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle+90, Vector3.forward);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {//Anything that projectiles shouldn't interact with
        if(collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node" && collision.gameObject.tag != "PickupItem")
        {
            if (numBounces >= maxBounces && previousObjectHit != collision.gameObject)
            {
                Destroy(gameObject);
            }

            numBounces++;
            Vector2 previousVelocity = rb.velocity * rb.velocity.magnitude; //Stop the object's velocity
            rb.Sleep();
            transform.Rotate(0,0,90); //TODO update this

            if (collision.gameObject.tag == "Left Wall" || collision.gameObject.tag == "Right Wall")
            {
                rb.AddForce(new Vector2(-previousVelocity.x,previousVelocity.y));
                previousObjectHit = collision.gameObject;
            }
            else if (collision.gameObject.tag == "Up Wall" || collision.gameObject.tag == "Down Wall")
            {
                rb.AddForce(new Vector2(previousVelocity.x, -previousVelocity.y));
                previousObjectHit = collision.gameObject;
            }
        }
    }*/

    public void SetHomingShots(bool value)
    {
        isHoming = value;
    }

    public bool hasHomingShots()
    {
        return isHoming;
    }

    public void SetBounces(int bounces)
    {
        maxBounces = bounces;
    }
}

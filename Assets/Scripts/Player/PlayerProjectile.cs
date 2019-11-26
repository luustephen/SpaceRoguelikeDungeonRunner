using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    public float radiusOfLockOn = 5;
    public float forceOfLockOn = 10;
    private Rigidbody2D rb;
    private EnemySpawner enemySpawnScript;
    private bool isHoming = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        enemySpawnScript = FindObjectOfType<EnemySpawner>();
	}
	
	// Update is called once per frame
	void Update () {

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

    private void OnTriggerEnter2D(Collider2D collision)
    {//Anything that projectiles shouldn't interact with
        if(collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node" && collision.gameObject.tag != "PickupItem")
        {
            print(collision.gameObject.tag);
            Destroy(gameObject);
        }
    }

    public void SetHomingShots(bool value)
    {
        isHoming = value;
    }

    public bool hasHomingShots()
    {
        return isHoming;
    }
}

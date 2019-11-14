using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    public float radiusOfLockOn = 5;
    public float forceOfLockOn = 10;
    private Rigidbody2D rb;
    private EnemySpawner enemySpawnScript;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        enemySpawnScript = FindObjectOfType<EnemySpawner>();
	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] nearbyObjects = enemySpawnScript.enemies;
        for(int i = 0; i < nearbyObjects.Length; i++)
        {
            if ((nearbyObjects[i].transform.position.y - transform.position.y) < radiusOfLockOn && (nearbyObjects[i].transform.position.x - transform.position.x) < radiusOfLockOn)
            {
                rb.AddForce((nearbyObjects[i].transform.position - transform.position) * forceOfLockOn);
                print("hello");
                continue;
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
}

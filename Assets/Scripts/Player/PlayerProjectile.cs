using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

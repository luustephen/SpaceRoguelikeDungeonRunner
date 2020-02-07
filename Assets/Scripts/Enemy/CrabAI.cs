using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAI : MonoBehaviour {

    private GameObject player;
    private Room room; //Room that the enemy resides in
    public bool moveHorizontal = true;
    private bool firstpass = true;
    public float speed = .05f; 

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        if(speed != 0)
        {
            speed = 1 / speed;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (firstpass)
        {
            if (!transform.parent)
            {
                Destroy(gameObject);
            }
            else
            {
                room = transform.parent.GetComponent<Room>();
            }
            firstpass = false;
        }
        if (player && room && room.InsideRoom(player))
        {
            float difference; //Difference between the player position and crab, can be x/y difference depending on moveHorizontal
            if (moveHorizontal)
            {
                difference = player.transform.position.x - transform.position.x;
                transform.Translate(new Vector3(difference/speed, 0, 0));
            }
            else
            {
                difference = player.transform.position.y - transform.position.y;
                transform.Translate(new Vector3(0, difference/speed, 0));
            }
        }
	}

    float GetDistanceToPlayer()
    {
        float distance = Mathf.Pow(player.transform.position.y - transform.position.y,2) + Mathf.Pow(player.transform.position.x - transform.position.x, 2);
        return distance;
    }
}

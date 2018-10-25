using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLighting : MonoBehaviour {

    private GameObject player;
    private Light roomLight;
    private Transform leftmostWall, rightmostWall, upmostWall, downmostWall;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        roomLight = transform.Find("room light").GetComponent<Light>();
        leftmostWall = transform.Find("leftmost wall");
        rightmostWall = transform.Find("rightmost wall");
        upmostWall = transform.Find("upmost wall");
        downmostWall = transform.Find("downmost wall");
        print(leftmostWall.transform.position.x + " " + rightmostWall.transform.position.x + " " + upmostWall.transform.position.x + " " + downmostWall.transform.position.x);
    }
	
	// Update is called once per frame
	void Update () {
		if(player.transform.position.x > leftmostWall.transform.position.x && player.transform.position.x < rightmostWall.transform.position.x && player.transform.position.y > downmostWall.transform.position.y && player.transform.position.y < upmostWall.transform.position.y)
        {
            roomLight.enabled = true;
            print(leftmostWall.transform.position.x + " " + rightmostWall.transform.position.x + " " + upmostWall.transform.position.x + " " + downmostWall.transform.position.x);
            print(player.transform.position.x + " " + player.transform.position.y);
        }
        else
        {
            roomLight.enabled = false;
        }
	}
}

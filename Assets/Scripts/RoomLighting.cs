using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLighting : MonoBehaviour {

    private GameObject[] players;
    private Light roomLight;
    private Transform leftmostWall, rightmostWall, upmostWall, downmostWall;
    private int numPlayers;

    // Use this for initialization
    void Start() {
        numPlayers = 1;                                 //Change for multiplayer
        for (int i = 0; i < numPlayers; i++) { 
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        roomLight = transform.Find("room light").GetComponent<Light>();
        leftmostWall = transform.Find("leftmost wall");
        rightmostWall = transform.Find("rightmost wall");
        upmostWall = transform.Find("upmost wall");
        downmostWall = transform.Find("downmost wall");
        print(leftmostWall.transform.position.x + " " + rightmostWall.transform.position.x + " " + upmostWall.transform.position.x + " " + downmostWall.transform.position.x);
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i].transform.position.x > leftmostWall.transform.position.x && players[i].transform.position.x < rightmostWall.transform.position.x && players[i].transform.position.y > downmostWall.transform.position.y && players[i].transform.position.y < upmostWall.transform.position.y)
            {
                roomLight.enabled = true;
                //print(leftmostWall.transform.position.x + " " + rightmostWall.transform.position.x + " " + upmostWall.transform.position.x + " " + downmostWall.transform.position.x);
                //print(players[i].transform.position.x + " " + players[i].transform.position.y);
            }
            else
            {
                roomLight.enabled = false;
            }
        }
	}
}

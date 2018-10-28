using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLighting : MonoBehaviour {

    private GameObject[] players;
    private GameObject[] roomLight;
    private Transform leftmostWall, rightmostWall, upmostWall, downmostWall;
    private GameObject floor;
    private int numPlayers;
    private int numLights;
    LightmapData[] lightmap_data;

    // Use this for initialization
    void Start() {
        numPlayers = 1;                                 //Change for multiplayer
        numLights = 4;
        roomLight = new GameObject[numLights];
        lightmap_data = LightmapSettings.lightmaps;
        for (int i = 0; i < numPlayers; i++) { 
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        floor = transform.Find("floor").gameObject;
        leftmostWall = transform.Find("leftmost wall");
        rightmostWall = transform.Find("rightmost wall");
        upmostWall = transform.Find("upmost wall");
        downmostWall = transform.Find("downmost wall");
        print(leftmostWall.transform.position.x + " " + rightmostWall.transform.position.x + " " + upmostWall.transform.position.x + " " + downmostWall.transform.position.x);
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].transform.position.x > leftmostWall.transform.position.x && players[i].transform.position.x < rightmostWall.transform.position.x && players[i].transform.position.y > downmostWall.transform.position.y && players[i].transform.position.y < upmostWall.transform.position.y)
            {
                foreach (Transform child in transform) if (child.CompareTag("Light"))
                    {
                        child.gameObject.GetComponent<Light>().enabled = true;
                    }
                floor.layer = 9;
            }
            else
            {
                foreach (Transform child in transform) if (child.CompareTag("Light"))
                    {
                        child.gameObject.GetComponent<Light>().enabled = false;
                    }
                floor.layer = 10;
            }
        }
	}
}

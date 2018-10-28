using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLighting : MonoBehaviour {

    private GameObject[] players;
    private Transform leftmostWall, rightmostWall, upmostWall, downmostWall;
    private GameObject floor;
    private int numPlayers;
    public int layerLightsAmbient, layerLightsOn, layerLightsOff;
    private bool explored;
    public float maxLight;
    private Light lightSource;
    private bool lightOn;

    // Use this for initialization
    void Start()
    {
        explored = false;
        numPlayers = 1;                                 //Change for multiplayer
        lightSource = GameObject.Find("Room Light On").GetComponent<Light>();
        for (int i = 0; i < numPlayers; i++)
        { 
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        floor = transform.Find("floor").gameObject;
        leftmostWall = transform.Find("leftmost wall");
        rightmostWall = transform.Find("rightmost wall");
        upmostWall = transform.Find("upmost wall");
        downmostWall = transform.Find("downmost wall");
    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < players.Length; i++)    //Check if Player is inside room or not, if so turn on lights, if already explored used ambient, if not use darkness
        {
            if (players[i].transform.position.x > leftmostWall.transform.position.x && players[i].transform.position.x < rightmostWall.transform.position.x && players[i].transform.position.y > downmostWall.transform.position.y && players[i].transform.position.y < upmostWall.transform.position.y)
            {
                floor.layer = layerLightsOn;
                if(!lightOn)
                    StartCoroutine("TurnOn");
                explored = true;
            }
            else
            {
                lightOn = false;
                if(explored)
                    floor.layer = layerLightsAmbient;
                else
                    floor.layer = layerLightsOff;
            }
        }
	}

    IEnumerator TurnOn()                //Slowly turn on lights when entering a room
    {
        lightSource.intensity = .1f;
        while (lightSource.intensity < maxLight)
        {
            lightSource.intensity += .05f;
            yield return new WaitForSeconds(.01f);
        }
        lightOn = true;
    }
}

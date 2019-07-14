using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private GameObject[] players;
    private GameObject[] doors;
    private GameObject[] enemies;
    private Transform leftmostWall, rightmostWall, upmostWall, downmostWall;
    private GameObject floor;
    private int numPlayers;
    public int layerLightsAmbient, layerLightsOn, layerLightsOff;
    private bool explored;
    public float maxLight;
    private Light lightSource;
    private bool lightOn;
    private bool firstpass = true;
    private GameObject[] enemiesInRoomList;
    public int maxEnemiesInRoom;
    public bool shouldSpawnEnemies;
    private Transform[] children;
    private Material[] childMaterials;
    public Material transparent;

    // Use this for initialization
    void Start()
    {
        doors = new GameObject[4];
        explored = false;
        numPlayers = 1;                                 //Change for multiplayer
        lightSource = GameObject.Find("Room Light On").GetComponent<Light>();
        for (int i = 0; i < numPlayers; i++)
        { 
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        foreach (Transform child in transform) //Get the doors for this room
        {
            if (child.tag == "Up Door")
            {
                doors[0] = child.gameObject;
            }
            if (child.tag == "Down Door")
            {
                doors[1] = child.gameObject;
            }
            if (child.tag == "Left Door")
            {
                doors[2] = child.gameObject;
            }
            if (child.tag == "Right Door")
            {
                doors[3] = child.gameObject;
            }
        }

        floor = transform.Find("floor").gameObject;
        leftmostWall = transform.Find("leftmost wall");
        rightmostWall = transform.Find("rightmost wall");
        upmostWall = transform.Find("upmost wall");
        downmostWall = transform.Find("downmost wall");
        enemiesInRoomList = new GameObject[maxEnemiesInRoom];

    }
	
	// Update is called once per frame
	void Update ()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); //Get list of enemies
        if (firstpass)
        {
            int enemyCounter = 0;
            for (int x = 0; x < enemies.Length; x++)
            {
                if (enemies[x].transform.position.x > leftmostWall.transform.position.x && enemies[x].transform.position.x < rightmostWall.transform.position.x && enemies[x].transform.position.y > downmostWall.transform.position.y && enemies[x].transform.position.y < upmostWall.transform.position.y)
                {
                    enemies[x].transform.SetParent(gameObject.transform);
                    if (maxEnemiesInRoom > 0 && enemyCounter < maxEnemiesInRoom) //Keep track of enemies in room and who they are
                    {
                        enemiesInRoomList[enemyCounter++] = enemies[x];
                    }
                }
            }

            children = new Transform[transform.childCount];
            childMaterials = new Material[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) //Put all the children in a list for easy access
            {
                SpriteRenderer sprite;
                children[i] = transform.GetChild(i);
                if(sprite = children[i].GetComponent<SpriteRenderer>()) //Store the child material to return after no longer transparent
                {
                    childMaterials[i] = sprite.material;
                }
            }
        }

        for (int i = 0; i < players.Length; i++)    //Check if Player is inside room or not, if so turn on lights, if already explored used ambient, if not use darkness
        {
            if (players[i].transform.position.x > leftmostWall.transform.position.x && players[i].transform.position.x < rightmostWall.transform.position.x && players[i].transform.position.y > downmostWall.transform.position.y && players[i].transform.position.y < upmostWall.transform.position.y)
            {

                for (int z = 0; z < transform.childCount; z++)
                {
                    if (children[z])
                    {
                        children[z].gameObject.layer = layerLightsOn;

                        SpriteRenderer sprite;
                        if (sprite = children[z].GetComponent<SpriteRenderer>()) //Set material to normal if player is in room
                        {
                            sprite.material = childMaterials[z];
                        }
                    }
                }
                //floor.layer = layerLightsOn;

                for (int a = 0; a < enemiesInRoomList.Length; a++)
                {
                    if (enemiesInRoomList[a] != null)
                        enemiesInRoomList[a].layer = layerLightsOn;
                }

                if (!lightOn)
                    StartCoroutine("TurnOn");
                explored = true;

                int enemiesInRoom = 0;

                for (int x = 0; x < enemies.Length; x++)
                {
                    if (enemies[x].transform.position.x > leftmostWall.transform.position.x && enemies[x].transform.position.x < rightmostWall.transform.position.x && enemies[x].transform.position.y > downmostWall.transform.position.y && enemies[x].transform.position.y < upmostWall.transform.position.y)
                    { //if enemy is in room with player
                        enemiesInRoom++;
                        enemies[x].transform.SetParent(gameObject.transform);
                        for (int k = 0; k < doors.Length; k++) //Lock the doors if enemies are alive
                        {
                            if (doors[k] != null && doors[k].GetComponent<DoorOpen>())
                                doors[k].GetComponent<DoorOpen>().shouldOpen = false;
                        }
                    }
                    else if (enemiesInRoom == 0) //Unlock if no enemies remain
                    {
                        for (int k = 0; k < doors.Length; k++)
                        {
                            if (doors[k] != null && doors[k].GetComponent<DoorOpen>())
                                doors[k].GetComponent<DoorOpen>().shouldOpen = true;
                        }
                    }
                }
            }
            else
            {
                lightOn = false;
                if (explored) //If room is already explored, put ambient lighting in it
                {
                    for (int z = 0; z < transform.childCount; z++)
                    {
                        if (children[z])
                        {
                            children[z].gameObject.layer = layerLightsAmbient;
                        }
                    }
                    //floor.layer = layerLightsAmbient;

                    for (int a = 0; a < enemiesInRoomList.Length; a++)
                    {
                        if(enemiesInRoomList[a] != null)
                            enemiesInRoomList[a].layer = layerLightsAmbient;
                    }
                }
                else //If room is not explored make it dark
                {
                    for (int z = 0; z < transform.childCount; z++)
                    {
                        if (children[z])
                        {
                            children[z].gameObject.layer = layerLightsOff;

                            SpriteRenderer sprite;
                            if (sprite = children[z].GetComponent<SpriteRenderer>()) //Set material to transparent if player isnt in room
                            {
                                sprite.material = transparent;
                            }
                        }
                        //floor.layer = layerLightsOff;
                        for (int a = 0; a < enemiesInRoomList.Length; a++)
                        {
                            if (enemiesInRoomList[a] != null)
                                enemiesInRoomList[a].layer = layerLightsOff;
                        }
                    }
                }
            }
        }
        firstpass = false;
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

    public bool InsideRoom(GameObject objectToCheck) //Whether an object is inside this room
    {
        if(objectToCheck.transform.position.x > leftmostWall.transform.position.x && objectToCheck.transform.position.x < rightmostWall.transform.position.x && objectToCheck.transform.position.y > downmostWall.transform.position.y && objectToCheck.transform.position.y < upmostWall.transform.position.y)
        {
            return true;
        }
        return false;
    }
}

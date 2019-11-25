using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] enemies; //All enemies possible
    private MapBuilder mapBuilder; //Map builder script
    private GameObject[] roomObjects; //All room objects in level
    private GameObject[] enemiesSpawned; //All enemies currently spawned

    private float dimensions; //Dimensions of each room, assumes square rooms


	// Use this for initialization
	void Start () {
        mapBuilder = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
        dimensions = mapBuilder.dimensions;
        roomObjects = mapBuilder.getAllRooms();
        enemiesSpawned = new GameObject[roomObjects.Length];
        if (mapBuilder)
        {
            if(enemies.Length > 0)
            {
                for (int i = 0; i < roomObjects.Length; i++)
                {
                    if (roomObjects[i] != null && roomObjects[i].GetComponent<Room>().shouldSpawnEnemies) //Check if the room exists and if you should spawn enemies in it
                    {
                        int rand = Random.Range(0,enemies.Length);
                        Vector3 position = new Vector3(roomObjects[i].transform.position.x + Random.Range(0,dimensions), roomObjects[i].transform.position.y + Random.Range(- dimensions / 2, dimensions / 2), roomObjects[i].transform.position.z); //Spawn somewhere within room kinda
                        enemies[rand].transform.SetPositionAndRotation(position,Quaternion.identity);
                        enemiesSpawned[i] = Instantiate(enemies[rand]); //spawn enemy inside room dimensions
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject[] getEnemies()
    {
        return enemiesSpawned;
    }
}

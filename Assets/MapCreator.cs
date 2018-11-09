using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {

   class Room{
        public GameObject location;
        public Transform[] leftDoors = new Transform[2];
        public Transform[] rightDoors = new Transform[2];
        public Transform[] upDoors = new Transform[2];
        public Transform[] downDoors = new Transform[2];
        public int exits;
    }

    public GameObject[] prebuiltRooms;
    private Room[] rooms; 
    public int numRooms;

    // Use this for initialization
    void Start()
    {
        rooms = new Room[numRooms];
        if (numRooms > 1) {
            for(int i = 0; i<numRooms; i++)
            {
                int numLeftDoors = 0;
                int numRightDoors = 0;
                int numUpDoors = 0;
                int numDownDoors = 0;
                foreach (Transform child in prebuiltRooms[i].GetComponent<Transform>())
                {
                    if (child.tag == "Up Door")
                        rooms[i].upDoors[numUpDoors++] = child;
                    if(child.tag == "Down Door")
                        rooms[i].downDoors[numDownDoors++] = child;
                    if (child.tag == "Left Door")
                        rooms[i].leftDoors[numLeftDoors++] = child;
                    if (child.tag == "Right Door")
                        rooms[i].rightDoors[numRightDoors++] = child;
                }
                rooms[i].exits = numLeftDoors + numRightDoors + numDownDoors + numUpDoors;
                rooms[i].location = prebuiltRooms[(int)Random.Range(0,prebuiltRooms.Length-1)]; //Change for seeding?
            }
            GameObject[] path = FindEssentialPath();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    GameObject[] FindEssentialPath()
    {
        GameObject[] path = new GameObject[numRooms / 4 * 3];
        path[0] = prebuiltRooms[0];
        path[path.Length - 1] = prebuiltRooms[4];
        for(int k = 1; k < path.Length-2; k++)
        {

        }
        return path;
    }
}

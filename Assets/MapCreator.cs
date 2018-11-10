using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{

    class Room
    {
        public GameObject roomObject;
        public Transform[] leftDoors = new Transform[2];
        public Transform[] rightDoors = new Transform[2];
        public Transform[] upDoors = new Transform[2];
        public Transform[] downDoors = new Transform[2];
        public int numLeftDoors, numRightDoors, numUpDoors, numDownDoors;
        public int exits;
        public bool isHallway;
    }

    public GameObject[] prebuiltRooms;
    private Room[] rooms;                           //Array that holds information about the gameobjects in prebuiltRooms
    public GameObject startingRoom;
    public GameObject endingRoom;
    public int numRooms;
    private int essentialPathLength;

    // Use this for initialization
    void Start()
    {
        rooms = new Room[prebuiltRooms.Length];
        if (numRooms > 1)
        {
            for (int i = 0; i < prebuiltRooms.Length; i++)
            {
                int numLeftDoors = 0;
                int numRightDoors = 0;
                int numUpDoors = 0;
                int numDownDoors = 0;

                rooms[i] = new Room();
                foreach (Transform child in prebuiltRooms[i].GetComponent<Transform>())
                {
                    if (child.tag == "Up Door")
                    {
                        rooms[i].upDoors[numUpDoors++] = child;
                        rooms[i].numUpDoors = numUpDoors;
                    }
                    if (child.tag == "Down Door")
                    {
                        rooms[i].downDoors[numDownDoors++] = child;
                        rooms[i].numDownDoors = numDownDoors;
                    }
                    if (child.tag == "Left Door")
                    {
                        rooms[i].leftDoors[numLeftDoors++] = child;
                        rooms[i].numLeftDoors = numLeftDoors;
                    }
                    if (child.tag == "Right Door")
                    {
                        rooms[i].rightDoors[numRightDoors++] = child;
                        rooms[i].numRightDoors = numRightDoors;
                    }
                }
                rooms[i].isHallway = (prebuiltRooms[i].tag == "Hallway") ? true : false;
                rooms[i].exits = numLeftDoors + numRightDoors + numDownDoors + numUpDoors;
                rooms[i].roomObject = prebuiltRooms[i];
            }
            GameObject[] path = FindEssentialPath();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject[] FindEssentialPath()
    {
        GameObject[] path = new GameObject[numRooms / 4 * 3];
        int currentNumRooms = numRooms - 2;
        path[0] = startingRoom;
        for (int k = 1; k < path.Length - 2; k++)
        {
            if (currentNumRooms > 0)
            {
                int randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                if (k % 2 == 1)
                {
                    while (!rooms[randomRoom].isHallway)
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                    path[k] = prebuiltRooms[randomRoom];    //Change for map seeding?
                    currentNumRooms = currentNumRooms - rooms[randomRoom].exits;
                }
                else
                {
                    while (rooms[randomRoom].isHallway)
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                    path[k] = prebuiltRooms[randomRoom];    //Change for map seeding?
                    currentNumRooms = currentNumRooms - rooms[randomRoom].exits;
                }
                essentialPathLength = k;
            }
            else
            {
                essentialPathLength = k;
                continue;
            }
        }
        path[essentialPathLength] = endingRoom;
        return path;
    }
}

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
        public Room[] upRooms = new Room[2];
        public Room[] downRooms = new Room[2];
        public Room[] leftRooms = new Room[2];
        public Room[] rightRooms = new Room[2];
    }

    public GameObject[] prebuiltRooms;              //Array of gameobjects for the editor
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
        GameObject[] path = new GameObject[numRooms / 4 * 3];       //Array of gameobjects for path objects
        Room[] pathRooms = new Room[numRooms / 4 * 3];              //Room information for path array
        int currentNumRooms = numRooms - 2;
        path[0] = prebuiltRooms[0];                                 //First room on array is starting room
        pathRooms[0] = rooms[0];                                    //Enter room information for starting room

        for (int k = 1; k < path.Length - 2; k++)                   //Iterate through path minus starting and ending rooms
        {
            if (currentNumRooms > 0)
            {
                int randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                if (k % 2 == 1)
                {
                    while (!rooms[randomRoom].isHallway || !checkRoomConnect(pathRooms[k-1],rooms[randomRoom]))    //Change room type until compatible hallway
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                    path[k] = prebuiltRooms[randomRoom];    //Change for map seeding?
                    pathRooms[k] = rooms[randomRoom];
                    connectRooms(pathRooms[k-1],pathRooms[k]);
                    currentNumRooms = currentNumRooms - rooms[randomRoom].exits;
                }
                else
                {
                    while (rooms[randomRoom].isHallway || rooms[randomRoom].exits < 2 || !checkRoomConnect(pathRooms[k-1], rooms[randomRoom]))      //For rooms
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                    path[k] = prebuiltRooms[randomRoom];    //Change for map seeding?
                    pathRooms[k] = rooms[randomRoom];
                    connectRooms(pathRooms[k - 1], pathRooms[k]);
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
        path[essentialPathLength] = endingRoom;        //Put the ending room onto the end of the path
        return path;
    }

    bool checkRoomConnect(Room first, Room second)          //Check if two rooms connect
    {
        if(first.numLeftDoors > 0 && second.numRightDoors > 0 || first.numRightDoors > 0 && second.numLeftDoors > 0 || first.numUpDoors > 0 && second.numDownDoors > 0 || first.numDownDoors > 0 && second.numUpDoors > 0)
            return true;
        else
            return false;
    }

    bool connectRooms(Room first, Room second)         //Connect the rooms together 
    {
        if (first.numLeftDoors > 0 && second.numRightDoors > 0)
        {
            for(int i = 0; i<first.leftRooms.Length-1; i++)
            {
                for (int k = 0; k < first.rightRooms.Length-1; k++)
                {
                    if (first.leftRooms[i] == null && second.rightRooms[k] == null)
                    {
                        first.leftRooms[i] = second;
                        second.rightRooms[k] = first;
                        return true;
                    }
                }
            }
        }
        if (first.numRightDoors > 0 && second.numLeftDoors > 0)
        {
            for (int i = 0; i < first.rightRooms.Length - 1; i++)
            {
                for (int k = 0; k < first.leftRooms.Length - 1; k++)
                {
                    if (first.rightRooms[i] == null && second.leftRooms[k] == null)
                    {
                        first.rightRooms[i] = second;
                        second.leftRooms[k] = first;
                        return true;
                    }
                }
            }
        }
        if (first.numUpDoors > 0 && second.numDownDoors > 0)
        {
            for (int i = 0; i < first.upRooms.Length - 1; i++)
            {
                for (int k = 0; k < first.downRooms.Length - 1; k++)
                {
                    if (first.upRooms[i] == null && second.downRooms[k] == null)
                    {
                        first.upRooms[i] = second;
                        second.downRooms[k] = first;
                        return true;
                    }
                }
            }
        }
        if (first.numDownDoors > 0 && second.numUpDoors > 0)
        {
            for (int i = 0; i < first.downRooms.Length - 1; i++)
            {
                for (int k = 0; k < first.upDoors.Length - 1; k++)
                {
                    if (first.downRooms[i] == null && second.upRooms[k] == null)
                    {
                        first.downRooms[i] = second;
                        second.upRooms[k] = first;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

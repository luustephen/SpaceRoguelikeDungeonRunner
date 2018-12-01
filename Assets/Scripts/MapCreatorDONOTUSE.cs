using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * OLD CODE DON'T USE, USE MAPBUILDER.CS INSTEAD, ONLY HERE FOR REFERENCE
 * 
 * 
*/
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

        public Room Copy()
        {
            Room copy = new Room();
            copy.roomObject = this.roomObject;
            copy.leftDoors = this.leftDoors;
            copy.rightDoors = this.rightDoors;
            copy.upDoors = this.upDoors;
            copy.downDoors = this.downDoors;
            copy.numDownDoors = this.numDownDoors;
            copy.numLeftDoors = this.numLeftDoors;
            copy.numRightDoors = this.numRightDoors;
            copy.numUpDoors = this.numUpDoors;
            copy.exits = this.exits;
            copy.isHallway = this.isHallway;
            copy.upRooms = new Room[2];
            copy.downRooms = new Room[2];
            copy.leftRooms = new Room[2];
            copy.rightRooms = new Room[2];
            return copy;
        }
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
        path[0] = Instantiate(prebuiltRooms[0]);                    //First room on array is starting room
        pathRooms[0] = rooms[0].Copy();                             //Enter room information for starting room

        for (int k = 1; k < path.Length - 2; k++)                   //Iterate through path minus starting and ending rooms
        {
            if (currentNumRooms > 0)
            {
                int randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                Room newRoom;
                GameObject temp;
                if (k % 2 == 1)
                {
                    newRoom = rooms[randomRoom].Copy();
                    while (!rooms[randomRoom].isHallway || !checkRoomConnect(pathRooms[k - 1], newRoom))
                    {    //Change room type until compatible hallway
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                        newRoom = rooms[randomRoom].Copy();
                    }
                    pathRooms[k] = newRoom;
                    path[k] = connectRooms(pathRooms[k - 1], pathRooms[k], Instantiate(prebuiltRooms[randomRoom]));    //Change for map seeding?
                    currentNumRooms = currentNumRooms - newRoom.exits;
                }
                else
                {
                    newRoom = rooms[randomRoom].Copy();
                    while (rooms[randomRoom].isHallway || rooms[randomRoom].exits < 2 || !checkRoomConnect(pathRooms[k - 1], newRoom))
                    {      //Change room type until compatible room
                        randomRoom = (int)Random.Range(0, prebuiltRooms.Length);
                        newRoom = rooms[randomRoom].Copy();
                    }
                    pathRooms[k] = newRoom;
                    path[k] = connectRooms(pathRooms[k - 1], pathRooms[k], Instantiate(prebuiltRooms[randomRoom]));    //Change for map seeding?
                    currentNumRooms = currentNumRooms - newRoom.exits;
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

    GameObject connectRooms(Room first, Room second, GameObject secondObject)         //Connect the rooms together 
    {
        float difference;
        if (first.numLeftDoors > 0 && second.numRightDoors > 0)
        {
            for(int i = 0; i<first.leftRooms.Length; i++)
            {
                for (int k = 0; k < first.rightRooms.Length; k++)
                {
                    if (first.leftRooms[i] == null && second.rightRooms[k] == null && i < first.numLeftDoors && k < second.numRightDoors)
                    {
                        first.leftRooms[i] = second;
                        second.rightRooms[k] = first;
                        difference = secondObject.transform.position.x - second.rightDoors[i].position.x;
                        secondObject.transform.position = new Vector3(first.leftDoors[i].position.x + difference, first.leftDoors[i].position.y, first.leftDoors[i].position.z);
                        secondObject.transform.position = first.downDoors[i].position;
                        return secondObject;
                    }
                }
            }
        }
        if (first.numRightDoors > 0 && second.numLeftDoors > 0)
        {
            for (int i = 0; i < first.rightRooms.Length; i++)
            {
                for (int k = 0; k < first.leftRooms.Length; k++)
                {
                    if (first.rightRooms[i] == null && second.leftRooms[k] == null && i < first.numRightDoors && k < second.numLeftDoors)
                    {
                        first.rightRooms[i] = second;
                        second.leftRooms[k] = first;
                        difference = secondObject.transform.position.x - second.leftDoors[i].position.x;
                        secondObject.transform.position = new Vector3(first.rightDoors[i].position.x + difference, first.rightDoors[i].position.y, first.rightDoors[i].position.z);
                        secondObject.transform.position = first.downDoors[i].position;
                        return secondObject;
                    }
                }
            }
        }
        if (first.numUpDoors > 0 && second.numDownDoors > 0)
        {
            for (int i = 0; i < first.upRooms.Length; i++)
            {
                for (int k = 0; k < first.downRooms.Length; k++)
                {
                    if (first.upRooms[i] == null && second.downRooms[k] == null && i < first.numUpDoors && k < second.numDownDoors)
                    {
                        first.upRooms[i] = second;
                        second.downRooms[k] = first;
                        difference = secondObject.transform.position.y - second.downDoors[i].position.y;
                        secondObject.transform.position =  new Vector3(first.upDoors[i].position.x, first.upDoors[i].position.y + difference, first.upDoors[i].position.z);
                        secondObject.transform.position = first.downDoors[i].position;
                        return secondObject;
                    }
                }
            }
        }
        if (first.numDownDoors > 0 && second.numUpDoors > 0)
        {
            for (int i = 0; i < first.downRooms.Length; i++)
            {
                for (int k = 0; k < first.upDoors.Length; k++)
                {
                    if (first.downRooms[i] == null && second.upRooms[k] == null && i < first.numDownDoors && k < second.numUpDoors)
                    {
                        first.downRooms[i] = second;
                        second.upRooms[k] = first;
                        difference = secondObject.transform.position.y - second.upDoors[i].position.y;
                        secondObject.transform.position = new Vector3(first.downDoors[i].position.x, first.downDoors[i].position.y + difference, first.downDoors[i].position.z);
                        secondObject.transform.position = first.downDoors[i].position;
                        return secondObject;
                    }
                }
            }
        }

        return null;
    }
}

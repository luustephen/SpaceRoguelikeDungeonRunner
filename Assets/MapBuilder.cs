using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour {

    class Room
    {
        public GameObject roomObject;
        public Room[] upRooms;
        public Room[] downRooms;
        public Room[] leftRooms;
        public Room[] rightRooms;
        public bool isHallway;

        public Room Copy()
        {
            Room temp = new Room();
            temp.roomObject = Instantiate(this.roomObject);
            temp.upRooms = new Room[this.upRooms.Length];
            temp.downRooms = new Room[this.downRooms.Length];
            temp.rightRooms = new Room[this.rightRooms.Length];
            temp.leftRooms = new Room[this.leftRooms.Length];
            temp.isHallway = this.isHallway;
            return temp;
        }
    }

    public GameObject[] prebuiltRooms;
    private Room[] rooms;
    public int numRooms;

    // Use this for initialization
    void Start () {
        rooms = new Room[prebuiltRooms.Length];
        for(int i = 0; i < rooms.Length; i++)
        {
            rooms[i] = new Room();
        }
        if (numRooms > 1)
        {
            for (int i = 0; i < prebuiltRooms.Length; i++)
            {
                rooms[i].roomObject = prebuiltRooms[i];
            }
        }
        FindEssentialPath(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FindEssentialPath(int start)
    {
        Room[] path;
        for (int i = 0; i < prebuiltRooms.Length; i++)
        {
            int numUpDoors = 0;
            int numDownDoors = 0;
            int numLeftDoors = 0;
            int numRightDoors = 0;
            foreach (Transform child in prebuiltRooms[i].GetComponent<Transform>())
            {
                if (child.tag == "Up Door")
                {
                    numUpDoors++;
                }
                if (child.tag == "Down Door")
                {
                    numDownDoors++;
                }
                if (child.tag == "Left Door")
                {
                    numLeftDoors++;
                }
                if (child.tag == "Right Door")
                {
                    numRightDoors++;
                }
            }
            rooms[i].isHallway = (prebuiltRooms[i].tag == "Hallway") ? true : false;
            rooms[i].leftRooms = new Room[numLeftDoors];
            rooms[i].rightRooms = new Room[numRightDoors];
            rooms[i].downRooms = new Room[numDownDoors];
            rooms[i].upRooms = new Room[numUpDoors];
        }

        int essentialPathLength = numRooms / 4 * 3;
        path = new Room[essentialPathLength];
        path[0] = rooms[start].Copy();

        for (int i = 1; i < essentialPathLength; i++)
        {
            int random = (int)Random.Range(0, prebuiltRooms.Length);
            Room newRoom = rooms[random].Copy();
            if (i % 2 == 1)
            {
                while (checkRoomExits(newRoom) < 2 || !newRoom.isHallway || !checkRoomConnect(path[i - 1], newRoom))     //If room is a hallway and can connect to the old room and has more than 2 exits
                {
                    Destroy(newRoom.roomObject);
                    random = (int)Random.Range(0, prebuiltRooms.Length);
                    newRoom = rooms[random].Copy();
                }
                path[i] = newRoom;
            }
            else
            {
                while (checkRoomExits(newRoom) < 2 || newRoom.isHallway || !checkRoomConnect(path[i - 1], newRoom))   //If a room is a room and can connect to the old room and has more than 2 exits
                {
                    Destroy(newRoom.roomObject);
                    random = (int)Random.Range(0, prebuiltRooms.Length);
                    newRoom = rooms[random].Copy();
                }
                path[i] = newRoom;
            }
            //Instantiate(path[i].roomObject);
        }
    }

    bool checkRoomConnect(Room first, Room second)
    {
        if(first.leftRooms.Length > 0 && second.rightRooms.Length > 0)
        {
            for(int i = 0; i < first.leftRooms.Length; i++)
            {
                for(int k = 0; k < second.rightRooms.Length; k++)
                {
                    if(first.leftRooms[i] == null && second.rightRooms[k] == null)
                    {
                        first.leftRooms[i] = second;
                        second.rightRooms[k] = first;
                        moveRoom(first, second, 0, 0);
                        return true;
                    }
                }
            }
        }
        if (first.rightRooms.Length > 0 && second.leftRooms.Length > 0)
        {
            for (int i = 0; i < first.rightRooms.Length; i++)
            {
                for (int k = 0; k < second.leftRooms.Length; k++)
                {
                    if (first.rightRooms[i] == null && second.leftRooms[k] == null)
                    {
                        first.rightRooms[i] = second;
                        second.leftRooms[k] = first;
                        moveRoom(first, second, 1, 0);
                        return true;
                    }
                }
            }
        }
        if (first.upRooms.Length > 0 && second.downRooms.Length > 0)
        {
            for (int i = 0; i < first.upRooms.Length; i++)
            {
                for (int k = 0; k < second.downRooms.Length; k++)
                {
                    if (first.upRooms[i] == null && second.downRooms[k] == null)
                    {
                        first.upRooms[i] = second;
                        second.downRooms[k] = first;
                        moveRoom(first, second, 2, 0);
                        return true;
                    }
                }
            }
        }
        if (first.downRooms.Length > 0 && second.upRooms.Length > 0)
        {
            for (int i = 0; i < first.downRooms.Length; i++)
            {
                for (int k = 0; k < second.upRooms.Length; k++)
                {
                    if (first.downRooms[i] == null && second.upRooms[k] == null)
                    {
                        first.downRooms[i] = second;
                        second.upRooms[k] = first;
                        moveRoom(first, second, 3, 0);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    int checkRoomExits(Room room)
    {
        return room.leftRooms.Length + room.rightRooms.Length + room.upRooms.Length + room.downRooms.Length;
    }

    void moveRoom(Room first, Room second, int direction, int doorNum)
    {
        int i = 0;
        foreach (Transform child in second.roomObject.transform)
        {
            if (child.tag == "Up Door" && direction == 3 && i++ == doorNum)
            {
                float difference = second.roomObject.transform.position.y - child.position.y;
                second.roomObject.transform.position = new Vector3(first.roomObject.transform.position.x, first.roomObject.transform.position.y + difference*2, first.roomObject.transform.position.z);
            }
            if (child.tag == "Down Door" && direction == 2 && i++ == doorNum)
            {
                float difference = second.roomObject.transform.position.y - child.position.y;
                second.roomObject.transform.position = new Vector3(first.roomObject.transform.position.x, first.roomObject.transform.position.y + difference*2, first.roomObject.transform.position.z);
            }
            if (child.tag == "Left Door" && direction == 1 && i++ == doorNum)
            {
                float difference = second.roomObject.transform.position.x - child.position.x;
                second.roomObject.transform.position = new Vector3(first.roomObject.transform.position.x + difference, first.roomObject.transform.position.y, first.roomObject.transform.position.z);
            }
            if (child.tag == "Right Door" && direction == 0 && i++ == doorNum)
            {
                float difference = second.roomObject.transform.position.x - child.position.x;
                second.roomObject.transform.position = new Vector3(first.roomObject.transform.position.x + difference, first.roomObject.transform.position.y, first.roomObject.transform.position.z);
            }
        }
    }
}

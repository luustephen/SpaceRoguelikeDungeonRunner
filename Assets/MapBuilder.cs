﻿using System.Collections;
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
    private Room[] roomInfo;
    private Room[] allRooms;
    public int numRooms;
    public int initialNumRooms;

    // Use this for initialization
    void Start () {
        roomInfo = new Room[prebuiltRooms.Length];
        allRooms = new Room[numRooms];
        for(int i = 0; i < roomInfo.Length; i++)
        {
            roomInfo[i] = new Room();
        }
        if (numRooms > 1)
        {
            for (int i = 0; i < prebuiltRooms.Length; i++)
            {
                roomInfo[i].roomObject = prebuiltRooms[i];
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
            roomInfo[i].isHallway = (prebuiltRooms[i].tag == "Hallway") ? true : false;
            roomInfo[i].leftRooms = new Room[numLeftDoors];
            roomInfo[i].rightRooms = new Room[numRightDoors];
            roomInfo[i].downRooms = new Room[numDownDoors];
            roomInfo[i].upRooms = new Room[numUpDoors];
        }

        int essentialPathLength = numRooms / 4 * 3;
        path = new Room[essentialPathLength];
        path[0] = roomInfo[start].Copy();
        allRooms[0] = path[start];

        for (int i = 1; i < essentialPathLength; i++)
        {
            int random = (int)Random.Range(0, initialNumRooms);
            print(random);
            Room newRoom = roomInfo[random].Copy();
            if (i % 2 == 1)
            {
                int x = 0;
                while (x++ < 100 && (checkRoomExits(newRoom) < 2|| !newRoom.isHallway || !checkRoomConnect(path[i - 1], newRoom) || roomIntersect(newRoom, i)))     //If room is a hallway and can connect to the old room and has more than 2 exits
                {
                    Destroy(newRoom.roomObject);
                    random = (int)Random.Range(0, initialNumRooms);
                    newRoom = roomInfo[random].Copy();
                }
                if (x >= 100 && i>1)
                {
                    i = i - 2;
                    deleteReferences(path[i+1]);
                    Destroy(path[i+1].roomObject);
                    path[i+1] = null;
                    allRooms[i+1] = null;
                }
                else
                {
                    path[i] = newRoom;
                    allRooms[i] = newRoom;
                }
            }
            else
            {
                int x = 0;
                while (x++ < 100 && (checkRoomExits(newRoom) < 2 || newRoom.isHallway || !checkRoomConnect(path[i - 1], newRoom) || roomIntersect(newRoom, i)))   //If a room is a room and can connect to the old room and has more than 2 exits
                {
                    Destroy(newRoom.roomObject);
                    random = (int)Random.Range(0, initialNumRooms);
                    newRoom = roomInfo[random].Copy();
                }
                if (x >= 100 && i>1)
                {
                    i = i - 2;
                    deleteReferences(path[i+1]);
                    Destroy(path[i+1].roomObject);
                    path[i+1] = null;
                    allRooms[i+1] = null;
                }
                else
                {
                    path[i] = newRoom;
                    allRooms[i] = newRoom;
                }
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

    bool roomIntersect(Room first, Room second)
    {

        float leftPosF = first.roomObject.transform.Find("leftmost wall").position.x;
        float rightPosF = first.roomObject.transform.Find("rightmost wall").position.x;
        float downPosF = first.roomObject.transform.Find("downmost wall").position.y;
        float upPosF = first.roomObject.transform.Find("upmost wall").position.y;
        float leftPosS = second.roomObject.transform.Find("leftmost wall").position.x;
        float rightPosS = second.roomObject.transform.Find("rightmost wall").position.x;
        float downPosS = second.roomObject.transform.Find("downmost wall").position.y;
        float upPosS = second.roomObject.transform.Find("upmost wall").position.y;

        if (leftPosF > leftPosS && leftPosF < rightPosS && upPosF < upPosS && upPosF > downPosS)
        {
            print("left up");
            return true;
        }
        if (leftPosF > leftPosS && leftPosF < rightPosS && downPosF < upPosS && downPosF > downPosS)
        {
            print("left down");
            return true;
        }
        if (rightPosF > leftPosS && rightPosF < rightPosS && upPosF < upPosS && upPosF > downPosS)
        {
            print("right up");
            return true;
        }
        if (rightPosF > leftPosS && rightPosF < rightPosS && downPosF < upPosS && downPosF > downPosS)
        {
            print("right down");
            return true;
        }

        return false;
    }

    bool roomIntersect(Room first, int arrayNum)
    {
        Room second;
        int offset = 1;

        float leftPosF = first.roomObject.transform.Find("leftmost wall").position.x;
        float rightPosF = first.roomObject.transform.Find("rightmost wall").position.x;
        float downPosF = first.roomObject.transform.Find("downmost wall").position.y;
        float upPosF = first.roomObject.transform.Find("upmost wall").position.y;
        float leftPosS, rightPosS, downPosS, upPosS;

        for (int i = 0; i < allRooms.Length; i++) {
            if (allRooms[i] != null && i != arrayNum - 1) {
                second = allRooms[i];
                leftPosS = second.roomObject.transform.Find("leftmost wall").position.x;
                rightPosS = second.roomObject.transform.Find("rightmost wall").position.x;
                downPosS = second.roomObject.transform.Find("downmost wall").position.y;
                upPosS = second.roomObject.transform.Find("upmost wall").position.y;

                if (leftPosF + offset > leftPosS && leftPosF + offset < rightPosS && upPosF - offset < upPosS && upPosF - offset > downPosS)
                {
                    print("left up");
                    return true;
                }
                if (leftPosF + offset > leftPosS && leftPosF + offset < rightPosS && downPosF + offset < upPosS && downPosF + offset > downPosS)
                {
                    print("left down");
                    return true;
                }
                if (rightPosF - offset > leftPosS && rightPosF - offset < rightPosS && upPosF - offset < upPosS && upPosF - offset > downPosS)
                {
                    print("right up");
                    return true;
                }
                if (rightPosF - offset > leftPosS && rightPosF - offset < rightPosS && downPosF + offset < upPosS && downPosF + offset > downPosS)
                {
                    print("right down");
                    return true;
                }
            }
        }
        return false;
    }

    void deleteReferences(Room first)
    {
        for(int i = 0; i<first.leftRooms.Length; i++)
        {
            if(first.leftRooms[i] != null)
            {
                for(int k = first.leftRooms[i].rightRooms.Length-1; k>=0; k--)
                {
                    if(first.leftRooms[i].rightRooms[k] != null && first.leftRooms[i].rightRooms[k] == first)
                    {
                        first.leftRooms[i].rightRooms[k] = null;
                    }   
                }
            }
        }

        for (int i = 0; i < first.rightRooms.Length; i++)
        {
            if (first.rightRooms[i] != null)
            {
                for (int k = first.rightRooms[i].leftRooms.Length-1; k >= 0; k--)
                {
                    if (first.rightRooms[i].leftRooms[k] != null && first.rightRooms[i].leftRooms[k] == first)
                    {
                        first.rightRooms[i].leftRooms[k] = null;
                    }
                }
            }
        }

        for (int i = 0; i < first.upRooms.Length; i++)
        {
            if (first.upRooms[i] != null)
            {
                for (int k = first.upRooms[i].downRooms.Length-1; k >= 0; k--)
                {
                    if (first.upRooms[i].downRooms[k] != null && first.upRooms[i].downRooms[k] == first)
                    {
                        first.upRooms[i].downRooms[k] = null;
                    }
                }
            }
        }

        for (int i = 0; i < first.downRooms.Length; i++)
        {
            if (first.downRooms[i] != null)
            {
                for (int k = first.downRooms[i].upRooms.Length-1; k >= 0; k--)
                {
                    if (first.downRooms[i].upRooms[k] != null && first.downRooms[i].upRooms[k] == first)
                    {
                        first.downRooms[i].upRooms[k] = null;
                    }
                }
            }
        }

    }
}

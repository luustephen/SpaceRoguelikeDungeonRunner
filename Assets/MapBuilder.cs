using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    /* Door Codes
     *     UDLR
     * 0 - 0000 None
     * 1 - 0001 R
     * 2 - 0010 L
     * 3 - 0011 LR
     * 4 - 0100 D
     * 5 - 0101 DR
     * 6 - 0110 DL
     * 7 - 0111 DLR
     * 8 - 1000 U
     * 9 - 1001 UR
     * 10- 1010 UL
     * 11- 1011 ULR
     * 12- 1100 UD
     * 13- 1101 UDR
     * 14- 1110 UDL
     * 15- 1111 UDLR
     */
    class Room
    {
        public int door;
        public float x;
        public float y;
        public bool isHallway;
        public GameObject roomObject;

        /*public Room()
        {
            door = 0;
            x = 0;
            y = 0;
            isHallway = false;
            roomObject = null;
        }*/

        public Room Copy()
        {
            Room copy = new Room();
            copy.door = this.door;
            copy.x = this.x;
            copy.y = this.y;
            copy.isHallway = this.isHallway;
            copy.roomObject = Instantiate(this.roomObject);
            return copy;
        }
    }

    public GameObject[] prebuiltRooms;
    private Room[] rooms;
    private Room[] essentialPath;
    private Room[] allRooms;
    public int numRooms;
    public int essentialPathLength;
    public int startingRoom;



    // Use this for initialization
    void Start()
    {
        rooms = new Room[prebuiltRooms.Length];
        for (int i = 0; i < prebuiltRooms.Length; i++)
        {
            rooms[i] = new Room();
            foreach (Transform child in prebuiltRooms[i].GetComponent<Transform>())
            {
                if (child.tag == "Up Door")
                {
                    rooms[i].door = rooms[i].door | 8;
                }
                if (child.tag == "Down Door")
                {
                    rooms[i].door = rooms[i].door | 4;
                }
                if (child.tag == "Left Door")
                {
                    rooms[i].door = rooms[i].door | 2;
                }
                if (child.tag == "Right Door")
                {
                    rooms[i].door = rooms[i].door | 1;
                }
            }
            rooms[i].isHallway = (prebuiltRooms[i].tag == "Hallway") ? true : false;
            rooms[i].roomObject = prebuiltRooms[i];
        }

        createEssentialPath();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void createEssentialPath()
    {
        if (essentialPathLength > numRooms)
        {//If essential path is longer than total number of rooms
            return;
        }
        essentialPath = new Room[essentialPathLength];
        allRooms = new Room[numRooms];
        essentialPath[0] = rooms[startingRoom].Copy();
        allRooms[0] = essentialPath[0];

        if (essentialPath[0].door == 8)//up
            checkRoom(0, 1, essentialPathLength-1);
        else if (essentialPath[0].door == 4)//down
            checkRoom(0, -1, essentialPathLength-1);
        else if (essentialPath[0].door == 2)//left
            checkRoom(-1, 0, essentialPathLength-1);
        else if (essentialPath[0].door == 1)//right
            checkRoom(1, 0, essentialPathLength-1);

        return;
    }

    bool checkRoom(int x, int y, int remainingLength)
    {
        if (remainingLength <= 0)
        {
            return true;
        }

        int[] possibleRooms = new int[16];
        int numPossibleRooms = 15;

        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i] != null)
            {
                if (allRooms[i].x == x && allRooms[i].y == y + 1)//disable up
                {
                    numPossibleRooms = numPossibleRooms & 7;
                }
                if (allRooms[i].x == x && allRooms[i].y == y - 1)//disable down
                {
                    numPossibleRooms = numPossibleRooms & 11;
                }
                if (allRooms[i].x == x - 1 && allRooms[i].y == y)//disable left
                {
                    numPossibleRooms = numPossibleRooms & 13;
                }
                if (allRooms[i].x == x + 1 && allRooms[i].y == y)//disable right if there is a room to the right
                {
                    numPossibleRooms = numPossibleRooms & 14;
                }
            }
        }

        int temp = 0;
        for (int i = 0; i < 16; i++)         //get an array of all possible permutations of the given room exits
        {
            if ((numPossibleRooms & i) == i)
            {
                possibleRooms[temp++] = i;
            }
            else
            {
                possibleRooms[temp++] = 0;
            }
        }

        int rand = Random.Range(0, 16);
        int dontInfLoop = 0;
        while (possibleRooms[rand] == 0 && dontInfLoop < 100)
        {
            rand = Random.Range(0, 16);
            dontInfLoop++;
        }
        GameObject tempRoom = null;
        int[] randList = new int[] { 1, 2, 4, 8 };
        int direction = randList[Random.Range(0,randList.Length)];
        dontInfLoop = 0;
        while((direction & numPossibleRooms) != direction && dontInfLoop < 100)
        {
            direction = randList[Random.Range(0, randList.Length)];
            dontInfLoop++;
        }

        if ((possibleRooms[rand] & direction) == direction)//create room
        {
            for (int k = 0; k < rooms.Length; k++)
            {
                if (rooms[k].door == numPossibleRooms)
                {
                    allRooms[essentialPath.Length - remainingLength] = rooms[k].Copy();
                    tempRoom = Instantiate(prebuiltRooms[k], new Vector3(x * 20.6f, y * 20.6f, 0), Quaternion.identity);
                }
            }
            if (allRooms[essentialPath.Length - remainingLength] == null)
            {
                Room aaa = new Room();
                aaa.door = numPossibleRooms;
                aaa.isHallway = false;
                aaa.roomObject = Instantiate(prebuiltRooms[0], new Vector3(x * 20.6f, y * 20.6f, 0), Quaternion.identity);

            }

            if (direction == 8)
            {
                if (checkRoom(x, y + 1, remainingLength - 1))
                    return true;
            }
            else if (direction == 4)
            {
                if (checkRoom(x, y - 1, remainingLength - 1))
                    return true;
            }

            else if (direction == 2)
            {
                if (checkRoom(x - 1, y, remainingLength - 1))
                    return true;
            }

            else if (direction == 1)
            {
                if (checkRoom(x + 1, y, remainingLength - 1))
                    return true;
            }
            else
            {
                Destroy(tempRoom);
                allRooms[essentialPath.Length - remainingLength] = null;
            }
        }

        return false;
    }
}

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
        public Room leftRoom;
        public Room rightRoom;
        public Room upRoom;
        public Room downRoom;

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
            checkRoom(0, 1, essentialPathLength - 1,4);
        else if (essentialPath[0].door == 4)//down
            checkRoom(0, -1, essentialPathLength - 1,8);
        else if (essentialPath[0].door == 2)//left
            checkRoom(-1, 0, essentialPathLength - 1,1);
        else if (essentialPath[0].door == 1)//right
            checkRoom(1, 0, essentialPathLength - 1,2);

        return;
    }

    bool checkRoom(int x, int y, int remainingLength, int needDoor)//this should probably be called create essential path recursive cause it does more than check/place 1 room
    {
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i] != null && allRooms[i].x == x && allRooms[i].y == y)
                return false;
        }

        if (remainingLength <= 0)
        {
            if (allRooms[essentialPathLength-1] != null)
                return true;
            return false;
        }

        int[] possibleRooms = new int[16];
        int numPossibleRooms = 15;

        for (int i = 0; i < allRooms.Length; i++)//Dont use rooms that try to open into another room without a corresponding door i.e. Room R and cant be put on the left of Room UD
        {
            if (allRooms[i] != null)
            {
                if (allRooms[i].x == x && allRooms[i].y == y + 1)//disable up
                {
                    if ((allRooms[i].door & 4) != 4)
                        numPossibleRooms = numPossibleRooms & 7;
                    else
                        needDoor = needDoor | 8;
                }
                if (allRooms[i].x == x && allRooms[i].y == y - 1)//disable down
                {
                    if ((allRooms[i].door & 8) != 8)
                        numPossibleRooms = numPossibleRooms & 11;
                    else
                        needDoor = needDoor | 4;
                }
                if (allRooms[i].x == x - 1 && allRooms[i].y == y)//disable left
                {
                    if ((allRooms[i].door & 1) != 1)
                        numPossibleRooms = numPossibleRooms & 13;
                    else
                        needDoor = needDoor | 2;
                }
                if (allRooms[i].x == x + 1 && allRooms[i].y == y)//disable right if there is a room to the right
                {
                    if ((allRooms[i].door & 2) != 2)
                        numPossibleRooms = numPossibleRooms & 14;
                    else
                        needDoor = needDoor | 1;
                }
            }
        }

        numPossibleRooms = numPossibleRooms | needDoor;//Add back the room that you came from into the possible rooms to choose
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
        while ((possibleRooms[rand] == 0 || possibleRooms[rand] == 8 || possibleRooms[rand] == 4 || possibleRooms[rand] == 2 || possibleRooms[rand] == 1) && dontInfLoop < 100)//get a random room from the list of possible rooms
        {
            rand = Random.Range(0, 16);
            dontInfLoop++;
        }
        rand = rand | needDoor;

        GameObject tempRoom = null;
        int[] randList = new int[] { 1, 2, 4, 8 };
        int direction = randList[Random.Range(0, randList.Length)];
        dontInfLoop = 0;
        while (((direction & numPossibleRooms) != direction || direction == needDoor || ((possibleRooms[rand] & direction) != direction)) && dontInfLoop < 100)//get a random direction from the four cardinal directions
        {
            direction = randList[Random.Range(0, randList.Length)];
            dontInfLoop++;
        }

        for (int k = 0; k < rooms.Length; k++)         //find the room with the corresponding room exits and slap it in there
        {
            if (rooms[k].door == possibleRooms[rand])
            {
                allRooms[essentialPath.Length - remainingLength] = rooms[k].Copy();
                allRooms[essentialPath.Length - remainingLength].roomObject.transform.position = new Vector3(x * 20.6f, y * 20.6f, 0);
                allRooms[essentialPath.Length - remainingLength].x = x;
                allRooms[essentialPath.Length - remainingLength].y = y;
                k = 9999;
            }
        }
        if (allRooms[essentialPath.Length - remainingLength] == null)   //if the room type doesn't exist then make the first room in the array
        {
            allRooms[essentialPath.Length - remainingLength] = rooms[0].Copy();
            allRooms[essentialPath.Length - remainingLength].roomObject.transform.position = new Vector3(x * 20.6f, y * 20.6f, 0);
            allRooms[essentialPath.Length - remainingLength].x = x;
            allRooms[essentialPath.Length - remainingLength].y = y;
            //Room aaa = rooms[0].Copy();
            //aaa.roomObject.transform.position = new Vector3(x * 20.6f, y * 20.6f, 0);

        }

        if ((possibleRooms[rand] & direction) == direction)//if the room and direction are possible in that room i.e. UDR can go down but can't go left
        {
            if (direction == 8)     //if we're going up create the room with y = y+1;
            {
                if (checkRoom(x, y + 1, remainingLength - 1,4))
                {
                    allRooms[essentialPathLength - remainingLength - 1].upRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].downRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                    Destroy(tempRoom);
            }
            else if (direction == 4)//down
            {
                if (checkRoom(x, y - 1, remainingLength - 1,8))
                {
                    allRooms[essentialPathLength - remainingLength - 1].downRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].upRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                    Destroy(tempRoom);
            }

            else if (direction == 2)//left
            {
                if (checkRoom(x - 1, y, remainingLength - 1,1))
                {
                    allRooms[essentialPathLength - remainingLength - 1].leftRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].rightRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                    Destroy(tempRoom);
            }

            else if (direction == 1)//right
            {
                if (checkRoom(x + 1, y, remainingLength - 1,2))
                {
                    allRooms[essentialPathLength - remainingLength - 1].rightRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].leftRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                    Destroy(tempRoom);
            }
            else                    //destroy the room if no upcoming room was placed
            {
                Destroy(tempRoom);
                allRooms[essentialPath.Length - remainingLength] = null;
            }
        }

        if(allRooms[essentialPathLength-remainingLength] == null)
        {

        }

        return false;
    }
}

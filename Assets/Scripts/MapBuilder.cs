using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const int UP = 8;
    public const int DOWN = 4;
    public const int LEFT = 2;
    public const int RIGHT = 1;
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
        public int door;//What kind of door ways it has, see above
        public int x;
        public int y;
        public bool isHallway; //Whether this room is a hallway or not, probably don't need this variable
        public GameObject roomObject; //Gameobject associated with the room
        public Room leftRoom; //THESE ROOM OBJECT POINTERS BECOME OBSOLETE AFTER THE CREATEROOMMAP() METHOD IS RUN, IF YOU NEED TO SEE NEARBY ROOMS USE room.door AND roomMap[]
        public Room rightRoom; //ONLY USED FOR ESSENTIAL PATH CREATION
        public Room upRoom; //Pointers to nearby rooms to be used for essential path creation
        public Room downRoom;

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

    public GameObject[] prebuiltRooms; //Set of rooms that have be built to have all the necessary components i.e. doors, walls, tags, etc.
    private Room[] rooms; //Set of room objects that are mapped to the same indicies as prebuilt rooms, holds room information gathered from the gameobject.
    private Room[] essentialPath; //Set or all essential path rooms
    private Room[] allRooms; //Set of all rooms
    private Room[,] roomMap; //2D array to visually represent the layout of rooms; is offset by x and y + essentialPathLength because arrays can't have negative values
    public float dimensions = 20.6f; //Dimensions for each room, assumes square rooms
    [Tooltip("Total number of rooms, must be larger than essential path")]
    public int numRooms; //Max number of rooms
    [Tooltip("Number of rooms to reach the final room")]
    public int essentialPathLength; //Length of the initial path built, minimum rooms to travel to end the level
    [Tooltip("What room should be used as the starting room")]
    public int startingRoom; //Index of starting room using prebuilt rooms array above



    // Use this for initialization
    void Awake()
    {
        rooms = new Room[prebuiltRooms.Length];
        for (int i = 0; i < prebuiltRooms.Length; i++)
        {
            rooms[i] = new Room();
            foreach (Transform child in prebuiltRooms[i].GetComponent<Transform>())
            {
                if (child.tag == "Up Door")
                {
                    rooms[i].door = rooms[i].door | UP;
                }
                if (child.tag == "Down Door")
                {
                    rooms[i].door = rooms[i].door | DOWN;
                }
                if (child.tag == "Left Door")
                {
                    rooms[i].door = rooms[i].door | LEFT;
                }
                if (child.tag == "Right Door")
                {
                    rooms[i].door = rooms[i].door | RIGHT;
                }
            }
            rooms[i].isHallway = (prebuiltRooms[i].tag == "Hallway") ? true : false;
            rooms[i].roomObject = prebuiltRooms[i];
        }

        CreateEssentialPath();
        CreateRoomMap();
        FinishPaths();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateEssentialPath()
    {
        if (essentialPathLength > numRooms)
        {//If essential path is longer than total number of rooms
            return;
        }
        essentialPath = new Room[essentialPathLength];
        allRooms = new Room[numRooms];
        essentialPath[0] = rooms[startingRoom].Copy();
        allRooms[0] = essentialPath[0];

        if (essentialPath[0].door == UP)//up
            CreateRoomEssential(0, 1, essentialPathLength - 1, DOWN);
        else if (essentialPath[0].door == DOWN)//down
            CreateRoomEssential(0, -1, essentialPathLength - 1, UP);
        else if (essentialPath[0].door == LEFT)//left
            CreateRoomEssential(-1, 0, essentialPathLength - 1, RIGHT);
        else if (essentialPath[0].door == 1)//right
            CreateRoomEssential(1, 0, essentialPathLength - 1, LEFT);

        return;
    }

    bool CreateRoomEssential(int x, int y, int remainingLength, int needDoor)//this should probably be called create essential path recursive cause it does more than check/place 1 room
    {
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i] != null && allRooms[i].x == x && allRooms[i].y == y)
                return false;
        }

        if (remainingLength <= 0)
        {
            if (allRooms[essentialPathLength - 1] != null)
                return true;
            return false;
        }

        int cameFrom = needDoor;        //Door that you came from
        int[] possibleRooms = new int[16];  //Array of possible rooms given the state of surrounding rooms
        int numPossibleRooms = 15; //Number of different room types, see room codes above

        for (int i = 0; i < allRooms.Length; i++)//Dont use rooms that try to open into another room without a corresponding door i.e. Room R and cant be put on the left of Room UD
        {
            if (allRooms[i] != null)
            {
                if (allRooms[i].x == x && allRooms[i].y == y + 1)//disable up door if there isn't a corresponding door, else enable up door
                {
                    if ((allRooms[i].door & DOWN) != DOWN)
                        numPossibleRooms = numPossibleRooms & 7;
                    else
                        needDoor = needDoor | UP;
                }
                if (allRooms[i].x == x && allRooms[i].y == y - 1)//disable down...
                {
                    if ((allRooms[i].door & UP) != UP)
                        numPossibleRooms = numPossibleRooms & 11;
                    else
                        needDoor = needDoor | DOWN;
                }
                if (allRooms[i].x == x - 1 && allRooms[i].y == y)//disable left...
                {
                    if ((allRooms[i].door & 1) != 1)
                        numPossibleRooms = numPossibleRooms & 13;
                    else
                        needDoor = needDoor | LEFT;
                }
                if (allRooms[i].x == x + 1 && allRooms[i].y == y)//disable right...
                {
                    if ((allRooms[i].door & LEFT) != LEFT)
                        numPossibleRooms = numPossibleRooms & 14;
                    else
                        needDoor = needDoor | RIGHT;
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
        while ((possibleRooms[rand] == 0 || possibleRooms[rand] == UP || possibleRooms[rand] == DOWN || possibleRooms[rand] == LEFT || possibleRooms[rand] == RIGHT) && dontInfLoop < 100)//get a random room from the list of all rooms
        {
            rand = Random.Range(0, 16);
            dontInfLoop++;
        }
        rand = rand | needDoor;

        GameObject tempRoom = null;
        int[] randList = new int[] { RIGHT, LEFT, DOWN, UP };
        int direction = randList[Random.Range(0, randList.Length)];
        dontInfLoop = 0;
        while (((direction & numPossibleRooms) != direction || direction == needDoor || ((possibleRooms[rand] & direction) != direction)) && dontInfLoop < 100)//get a random direction from the four cardinal directions and possible directions from numpossiblerooms
        {
            direction = randList[Random.Range(0, randList.Length)];
            dontInfLoop++;
        }

        for (int k = 0; k < rooms.Length; k++)         //find the room with the corresponding room exits and slap it in there
        {
            if (rooms[k].door == possibleRooms[rand])
            {
                allRooms[essentialPathLength - remainingLength] = rooms[k].Copy();
                allRooms[essentialPathLength - remainingLength].roomObject.transform.position = new Vector3(x * dimensions, y * dimensions, 0);
                allRooms[essentialPathLength - remainingLength].x = x;
                allRooms[essentialPathLength - remainingLength].y = y;
                if (cameFrom == UP)
                {
                    allRooms[essentialPathLength - remainingLength - 1].downRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].upRoom = allRooms[essentialPathLength - remainingLength - 1];
                }

                else if (cameFrom == DOWN)
                {
                    allRooms[essentialPathLength - remainingLength - 1].upRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].downRoom = allRooms[essentialPathLength - remainingLength - 1];
                }

                else if (cameFrom == LEFT)
                {
                    allRooms[essentialPathLength - remainingLength - 1].rightRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].leftRoom = allRooms[essentialPathLength - remainingLength - 1];
                }

                else if (cameFrom == RIGHT)
                {
                    allRooms[essentialPathLength - remainingLength - 1].leftRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].rightRoom = allRooms[essentialPathLength - remainingLength - 1];
                }
                k = 9999;
            }
        }

        if ((possibleRooms[rand] & direction) == direction)//if the room and direction are possible in that room i.e. UDR can go down but can't go left
        {
            if (direction == UP)     //if we're going up create the room with y = y+1;
            {
                if (CreateRoomEssential(x, y + 1, remainingLength - 1, DOWN))
                {
                    allRooms[essentialPathLength - remainingLength - 1].upRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].downRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                {
                    if (allRooms[essentialPathLength - remainingLength] != null)
                        Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                    allRooms[essentialPath.Length - remainingLength] = null;
                }
            }
            else if (direction == DOWN)//down
            {
                if (CreateRoomEssential(x, y - 1, remainingLength - 1, UP))
                {
                    allRooms[essentialPathLength - remainingLength - 1].downRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].upRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                {
                    if (allRooms[essentialPathLength - remainingLength] != null)
                        Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                    allRooms[essentialPath.Length - remainingLength] = null;
                }
            }

            else if (direction == LEFT)//left
            {
                if (CreateRoomEssential(x - 1, y, remainingLength - 1, RIGHT))
                {
                    allRooms[essentialPathLength - remainingLength - 1].leftRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].rightRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                {
                    if (allRooms[essentialPathLength - remainingLength] != null)
                        Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                    allRooms[essentialPath.Length - remainingLength] = null;
                }
            }

            else if (direction == RIGHT)//right
            {
                if (CreateRoomEssential(x + 1, y, remainingLength - 1, LEFT))
                {
                    allRooms[essentialPathLength - remainingLength - 1].rightRoom = allRooms[essentialPathLength - remainingLength];
                    allRooms[essentialPathLength - remainingLength].leftRoom = allRooms[essentialPathLength - remainingLength - 1];
                    return true;
                }
                else
                {
                    if (allRooms[essentialPathLength - remainingLength] != null)
                        Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                    allRooms[essentialPath.Length - remainingLength] = null;
                }
            }
            else                    //destroy the room if no upcoming room was placed
            {
                if (allRooms[essentialPathLength - remainingLength] != null)
                    Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                allRooms[essentialPath.Length - remainingLength] = null;
            }
        }

        if (allRooms[essentialPathLength - remainingLength] == null)
        {
            for (int k = 0; k < rooms.Length; k++)         //find the room with the max exits and slap it in there
            {
                if (rooms[k].door == numPossibleRooms)
                {
                    allRooms[essentialPathLength - remainingLength] = rooms[k].Copy();
                    allRooms[essentialPathLength - remainingLength].roomObject.transform.position = new Vector3(x * dimensions, y * dimensions, 0);
                    allRooms[essentialPathLength - remainingLength].x = x;
                    allRooms[essentialPathLength - remainingLength].y = y;
                    if ((numPossibleRooms & UP) == UP && cameFrom != UP && CreateRoomEssential(x, y + 1, remainingLength - 1, DOWN)) //try up room
                    {
                        allRooms[essentialPathLength - remainingLength - 1].upRoom = allRooms[essentialPathLength - remainingLength];
                        allRooms[essentialPathLength - remainingLength].downRoom = allRooms[essentialPathLength - remainingLength - 1];
                        return true;
                    }
                    else if ((numPossibleRooms & DOWN) == DOWN && cameFrom != DOWN && CreateRoomEssential(x, y - 1, remainingLength - 1, UP)) //try down
                    {
                        allRooms[essentialPathLength - remainingLength - 1].downRoom = allRooms[essentialPathLength - remainingLength];
                        allRooms[essentialPathLength - remainingLength].upRoom = allRooms[essentialPathLength - remainingLength - 1];
                        return true;
                    }
                    else if ((numPossibleRooms & LEFT) == LEFT && cameFrom != LEFT && CreateRoomEssential(x - 1, y, remainingLength - 1, RIGHT)) //try left
                    {
                        allRooms[essentialPathLength - remainingLength - 1].leftRoom = allRooms[essentialPathLength - remainingLength];
                        allRooms[essentialPathLength - remainingLength].rightRoom = allRooms[essentialPathLength - remainingLength - 1];
                        return true;
                    }
                    else if ((numPossibleRooms & RIGHT) == RIGHT && cameFrom != RIGHT && CreateRoomEssential(x + 1, y, remainingLength - 1, LEFT)) //try right
                    {
                        allRooms[essentialPathLength - remainingLength - 1].rightRoom = allRooms[essentialPathLength - remainingLength];
                        allRooms[essentialPathLength - remainingLength].leftRoom = allRooms[essentialPathLength - remainingLength - 1];
                        return true;
                    }
                    else
                    {
                        if (allRooms[essentialPathLength - remainingLength] != null)
                            Destroy(allRooms[essentialPathLength - remainingLength].roomObject);
                        allRooms[essentialPath.Length - remainingLength] = null;
                        return false;
                    }
                }
            }
        }

        return false; //If everything fails to make a room then this recursive path doesn't work
    }

    void FinishPaths()
    {
        int allRoomsIndex = essentialPathLength;
        for (int i = 0; i < allRooms.Length; i++)//Iterate through all the rooms
        {
            if(allRooms[i] != null && allRoomsIndex < numRooms)//Make sure the room exists to try to seal up and that we aren't over our max rooms
            {
                for(int direction = RIGHT; direction <= UP; direction *= 2)//Check all directions ...
                {
                    int x = allRooms[i].x + essentialPathLength;
                    int y = allRooms[i].y + essentialPathLength;
                    int needDoor = 0;
                    if((allRooms[i].door & direction) == direction)//... and see if the direction is compatible with the door layout i.e. ULR can go left
                    {
                        if (direction == UP && roomMap[x, y + 1] == null)//Move in the direction if there is a empty space in that direction
                        {
                            y++;
                            needDoor = needDoor | DOWN;
                        }
                        else if (direction == DOWN && roomMap[x, y - 1] == null)
                        {
                            y--;
                            needDoor = needDoor | UP;
                        }
                        else if (direction == RIGHT && roomMap[x + 1, y] == null)
                        {
                            x++;
                            needDoor = needDoor | LEFT;
                        }
                        else if (direction == LEFT && roomMap[x - 1, y] == null)
                        {
                            x--;
                            needDoor = needDoor | RIGHT;
                        }

                        if (needDoor != 0)// Make sure you dont go here if you dont need a new room
                        {
                            if (roomMap[x, y + 1] != null && (roomMap[x, y + 1].door & DOWN) == DOWN)//Check where the new room will be placed to see if surrounding rooms have doors that connect
                                needDoor = needDoor | UP;
                            if (roomMap[x, y - 1] != null && (roomMap[x, y - 1].door & UP) == UP)
                                needDoor = needDoor | DOWN;
                            if (roomMap[x + 1, y] != null && (roomMap[x + 1, y].door & LEFT) == LEFT)
                                needDoor = needDoor | RIGHT;
                            if (roomMap[x - 1, y] != null && (roomMap[x - 1, y].door & RIGHT) == RIGHT)
                                needDoor = needDoor | LEFT;

                            if (needDoor > 0 && needDoor < 16)
                            {
                                for (int k = 0; k < rooms.Length; k++)         //find the room and create it in the right place
                                {
                                    if (rooms[k].door == needDoor)
                                    {
                                        allRooms[allRoomsIndex] = rooms[k].Copy();
                                        roomMap[x, y] = allRooms[allRoomsIndex];
                                        x -= essentialPathLength;
                                        y -= essentialPathLength;
                                        allRooms[allRoomsIndex].x = x;
                                        allRooms[allRoomsIndex].y = y;
                                        allRooms[allRoomsIndex++].roomObject.transform.position = new Vector3(x * dimensions, y * dimensions, 0);
                                        k = 9999;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void CreateRoomMap()
    {
        roomMap = new Room[(essentialPathLength*2) + 1, (essentialPathLength*2) + 1];

        for (int i = 0; i < allRooms.Length; i++)    //Create 2d array representing the map
        {
            if (allRooms[i] != null)
                roomMap[(int)allRooms[i].x + essentialPathLength, (int)allRooms[i].y + essentialPathLength] = allRooms[i];
        }
    }

    public GameObject[] getAllRooms()
    {
        GameObject[] temp;
        if (allRooms != null)
        {
            temp = new GameObject[allRooms.Length];
            for (int i = 0; i < allRooms.Length; i++)
            {
                if (allRooms[i] != null) { 
                    temp[i] = allRooms[i].roomObject;
                    print(temp[i].transform.position.x + "," + temp[i].transform.position.y);
                    //temp[i].transform.Translate(Vector3.up * dimensions * 2);
                    //temp[i].transform.Translate(Vector3.left * dimensions * 2);
                }
            }
            return temp;
        }
        else
        {
            return null;
        }
    }
}

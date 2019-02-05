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
        public bool isHallway;
        public GameObject roomObject;
        public Room leftRoom;
        public Room rightRoom;
        public Room upRoom;
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

    public GameObject[] prebuiltRooms;
    private Room[] rooms;
    private Room[] essentialPath;
    private Room[] allRooms;
    private Room[,] roomMap;
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
        int numPossibleRooms = 15;

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
                allRooms[essentialPathLength - remainingLength].roomObject.transform.position = new Vector3(x * 20.6f, y * 20.6f, 0);
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
                    allRooms[essentialPathLength - remainingLength].roomObject.transform.position = new Vector3(x * 20.6f, y * 20.6f, 0);
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
        int x;
        int y;
        int newDoors = 0;
        for(int i = 0; i < allRooms.Length; i++)
        {
            if(allRooms[i] != null)
            {
                for(int k = 1; k <= 8; k *= 2)
                {
                    if(essentialPathLength < numRooms-1 && (allRooms[i].door & k) == k)//Check if there is a doorway in that direction and if we haven't hit max rooms
                    {
                        x = allRooms[i].x + essentialPathLength;
                        y = allRooms[i].y + essentialPathLength;

                        if (k == UP && allRooms[i].upRoom == null)//Check if the doorway is open
                        {
                            y++;
                        }
                        else if(k == DOWN && allRooms[i].downRoom == null)
                        {
                            y--;
                        }
                        else if(k == LEFT && allRooms[i].leftRoom == null)
                        {
                            x--;
                        }
                        else if(k == RIGHT && allRooms[i].rightRoom == null)
                        {
                            x++;
                        }

                        if (!(roomMap[x, (y + 1)] == null) && roomMap[x, (y + 1)].downRoom == null)
                            newDoors = newDoors | UP;
                        if (!(roomMap[x, (y - 1)] == null) && roomMap[x, (y - 1)].upRoom == null)
                            newDoors = newDoors | DOWN;
                        if (!(roomMap[(x + 1), y] == null) && roomMap[(x + 1), y].leftRoom == null)
                            newDoors = newDoors | RIGHT;
                        if (!(roomMap[(x - 1), y] == null) && roomMap[(x - 1), y].rightRoom == null)
                            newDoors = newDoors | LEFT;

                        for (int j = 0; j < rooms.Length; j++)
                        {
                            if (rooms[j].door == newDoors)
                            {
                                allRooms[++essentialPathLength] = rooms[j].Copy();
                                allRooms[essentialPathLength].roomObject.transform.position = new Vector3((x-essentialPathLength) * 20.6f, (y-essentialPathLength) * 20.6f, 0);
                                if(!(roomMap[x, (y + 1)] == null))
                                {
                                    allRooms[essentialPathLength].upRoom = roomMap[x, y + 1];
                                    roomMap[x, y + 1].downRoom = allRooms[essentialPathLength];
                                }
                                if (!(roomMap[x, (y - 1)] == null))
                                {
                                    allRooms[essentialPathLength].downRoom = roomMap[x, y - 1];
                                    roomMap[x, y - 1].upRoom = allRooms[essentialPathLength];
                                }
                                if (!(roomMap[(x - 1), y] == null))
                                {
                                    allRooms[essentialPathLength].leftRoom = roomMap[x-1, y];
                                    roomMap[x-1, y].rightRoom = allRooms[essentialPathLength];
                                }
                                if (!(roomMap[(x + 1), y] == null))
                                {
                                    allRooms[essentialPathLength].rightRoom = roomMap[x+1, y];
                                    roomMap[x+1, y].leftRoom = allRooms[essentialPathLength];
                                }
                                roomMap[x, y] = allRooms[essentialPathLength];
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
}

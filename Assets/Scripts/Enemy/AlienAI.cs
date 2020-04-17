using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAI : MonoBehaviour
{

    private GameObject player;
    private Room room; //Room that the enemy resides in
    private bool firstpass = true;
    public float speed = .1f;
    [Tooltip("Number of nodes in row per room, default 9")]
    public int numRowNodes = 9;
    [Tooltip("Number of nodes in column per room, default 9")]
    public int numColNodes = 9;
    private int numNodes = -1;
    private GameObject nodeMap;
    private GameObject[,] nodeArray;
    private GameObject start; //Starting node that this alien is in
    private GameObject goal; //Node that the player is in SET THIS LATER
    private GameObject nodeToMove; //Node to move towards player
    private int nodeToMoveIndex = 1;
    private PlayerMovement2 playerMovementScript;
    private bool shouldMove = true;
    private bool setANewStart = true; //Should we set a new start point for a*
    public float howCloseToGet = .5f; //How close to get to each node before moving to next
    private bool modifyMovement = false; //Whether the enemy has gotten stuck on an object, try to move elsewhere;
    private ContactPoint2D contactPoint; //If enemy hits something, this is where it hit it

    // Use this for initialization
    void Start()
    {
        numNodes = numColNodes * numColNodes;
        nodeArray = new GameObject[numRowNodes, numColNodes];
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovementScript = player.GetComponent<PlayerMovement2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstpass)
        {
            if (!transform.parent) //If the enemy isn't linked to a room then destroy the enemy
            {
                Destroy(gameObject);
            }
            else
            {
                room = transform.parent.GetComponent<Room>();
            }

            if (transform.parent && numNodes != -1) //Find the nodemap and put the nodes in an array
            {
                int i = 0;
                int k = 0;
                //nodeMap = GameObject.FindGameObjectWithTag("Node");
                foreach (Transform findNodeMap in transform.parent)
                {
                    if (findNodeMap.tag == "Node")
                    {
                        foreach (Transform child in findNodeMap) //Find all children nodes and put them into this array
                        {
                            if (i < numRowNodes)
                            {
                                nodeArray[i, k] = child.gameObject;
                                k = (k + 1) % numColNodes;
                                if (k == 0)
                                {
                                    i++;
                                }
                            }
                        }
                    }
                }
            }

            firstpass = false;
        }

        transform.rotation = new Quaternion(0, 0, 0, 0);

        if (start != null && Mathf.Abs(transform.position.x - start.transform.position.x) < howCloseToGet && Mathf.Abs(transform.position.y - start.transform.position.y) < howCloseToGet) //if close enough
        {
            setANewStart = true;
        }

        if (start && player && room && room.InsideRoom(player))
        {
            goal = playerMovementScript.node;
            if (goal == null)
            {
                goal = nodeArray[1, 1];
            }

            Dictionary<GameObject, int> frontier = new Dictionary<GameObject, int>();
            frontier[start] = 0;
            Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
            Dictionary<GameObject, int> costSoFar = new Dictionary<GameObject, int>();
            cameFrom[start] = null;
            costSoFar[start] = 0;

            int numIterations = 0;
            while (frontier.Count != 0)
            {
                GameObject current = null;
                int highestValue = -1;
                foreach (KeyValuePair<GameObject, int> node in frontier) //Find highest priority 
                {
                    if (node.Value > highestValue)
                    {
                        current = node.Key;
                        highestValue = node.Value;
                    }
                }

                if (current == goal || current == null)
                {
                    GameObject temp = current;
                    GameObject beforeTemp = null;
                    while (cameFrom[temp] != null)
                    {
                        beforeTemp = temp;
                        temp = cameFrom[temp];
                    }
                    if (beforeTemp != null)
                    {
                        nodeToMove = beforeTemp;
                    }
                    break;
                }

                if (current != null)
                {
                    frontier.Remove(current);
                }

                int x = 999; //x and y indices for current
                int y = 999; //basically where it is in nodeArray
                for (int i = 0; i < nodeArray.GetLength(0); i++) //rows
                {
                    for (int k = 0; k < nodeArray.GetLength(1); k++) //columns
                    {
                        if (current.transform.localPosition.x == nodeArray[i, k].transform.localPosition.x && current.transform.localPosition.y == nodeArray[i, k].transform.localPosition.y)
                        {
                            x = i;
                            y = k;
                        }
                    }
                }

                for (int dx = -1; dx < 2; dx++) //Check neighbors
                {
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (x != 999 && x + dx >= 0 && x + dx < numRowNodes && y + dy >= 0 && y + dy < numColNodes && !nodeArray[x + dx, y + dy].GetComponent<NodeScript>().occupied)
                        {
                            int newCost = costSoFar[current] + 1;
                            GameObject next = nodeArray[x + dx, y + dy];
                            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                            {
                                costSoFar[next] = newCost;
                                int priority = newCost + (int)GetDistanceToPlayerModified(next);
                                frontier[next] = priority;
                                cameFrom[next] = current;
                                /*if(numIterations == nodeToMoveIndex)
                                {
                                    nodeToMove = current;
                                }*/
                            }
                        }
                    }
                }
                numIterations++;
            }
        }

        /*NodeScript nodeScript = null;
        for(int i = 0; i < nodeArray.GetLength(0); i++) //rows
        {
            for (int k = 0; k < nodeArray.GetLength(1); k++) //columns
            {
                if (nodeArray[i, k] != null && (nodeScript = nodeArray[i,k].gameObject.GetComponent<NodeScript>())) //If the node exists and has a nodescript
                {

                }
            }
        }*/



        if (player && room && room.InsideRoom(player) && shouldMove && nodeToMove != null) //if the player is in the room and there is a node to move to
        {
            Vector3 normalizedDirection = (nodeToMove.transform.position - transform.position).normalized; //normalized direction to next node to move
            if (modifyMovement) //if the enemy is stuck on an object
            {
                if (Mathf.Abs(contactPoint.normal.y) > Mathf.Abs(contactPoint.normal.x)) //if we are further away vertically 
                {
                    float leftOrRight = (normalizedDirection.x > 0) ? 1 : -1;
                    transform.Translate(Vector3.right * speed * leftOrRight);
                    //print("leftright");
                }
                else //if we are further away horizontally
                {
                    float upOrDown = (normalizedDirection.y > 0) ? 1 : -1; //move up or down to unstuck us
                    transform.Translate(Vector3.up * speed * upOrDown);
                    //print("updown");
                }
            }
            else
            {
                transform.Translate(normalizedDirection * speed);
                //print(nodeToMove.transform.position + "!!");
            }
            //print(nodeToMove.transform.localPosition + "!!");
        }
    }

    float GetDistanceToPlayerModified(GameObject source)
    {
        float distance = Mathf.Pow(player.transform.position.y - source.transform.position.y, 2) + Mathf.Pow(player.transform.position.x - source.transform.position.x, 2);
        if(400-distance > 0)
            return 400 - distance;
        return 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Node" && setANewStart)
        {
            if (!collision.gameObject.GetComponent<NodeScript>().occupied)
            {
                start = collision.gameObject;
                setANewStart = false;
            }
        }
        if (collision.gameObject == nodeToMove)
        {
            shouldMove = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == nodeToMove)
        {
            shouldMove = false;
        }
        else
        {
            shouldMove = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            modifyMovement = true;
            contactPoint = collision.GetContact(0);
            print("COLLIDED");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            modifyMovement = false;
            print("UNCOLLIDED");
        }
    }
}
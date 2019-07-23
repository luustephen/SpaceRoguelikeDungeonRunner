using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAI : MonoBehaviour {

    private GameObject player;
    private Room room; //Room that the enemy resides in
    private bool firstpass = true;
    public float speed = .1f;
    [Tooltip("Number of nodes in row per room, default 90")]
    public int numRowNodes = 9;
    [Tooltip("Number of nodes in column per room, default 90")]
    public int numColNodes = 9;
    private int numNodes = -1;
    private GameObject nodeMap;
    private GameObject[] nodes;

    // Use this for initialization
    void Start()
    {
        nodes = new GameObject[numNodes];
        player = GameObject.FindGameObjectWithTag("Player");
        numNodes = numColNodes * numColNodes;
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
                nodeMap = GameObject.FindGameObjectWithTag("Node");
                foreach (Transform child in nodeMap.transform)
                {
                    if (i < numNodes)
                        nodes[i++] = child.gameObject;
                }
            }

            firstpass = false;
        }
        if (player && room && room.InsideRoom(player))
        {
            Vector3 normalizedDirection = (player.transform.position - transform.position).normalized;
            transform.Translate(normalizedDirection * speed);
            
        }
    }

    float GetDistanceToPlayer()
    {
        float distance = Mathf.Pow(player.transform.position.y - transform.position.y, 2) + Mathf.Pow(player.transform.position.x - transform.position.x, 2);
        return distance;
    }
}

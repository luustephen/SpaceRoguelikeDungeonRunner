using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    private GameObject[] players;
    private int numPlayers;
    public float openDistance;              //How close the player needs to be to open the door
    private Vector3 originalPosition;
    private Vector3 openedPosition;
    public float toOpenDistance;            //How far the door needs to move to open completely

    // Use this for initialization
    void Start()
    {
        originalPosition = transform.position;
        openedPosition = new Vector3(transform.position.x,(transform.position.y + toOpenDistance), transform.position.z);
        numPlayers = 1;                                 //Change for multiplayer
        for (int i = 0; i < numPlayers; i++)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            //print(Mathf.Abs(players[i].transform.position.x - transform.position.x));
            print(Mathf.Abs(players[i].transform.position.y - transform.position.y));
            if (Mathf.Abs(players[i].transform.position.x - originalPosition.x) < openDistance && Mathf.Abs(players[i].transform.position.y - originalPosition.y) < openDistance && transform.position.y  <= openedPosition.y)
            {
                transform.Translate(0, .1f, 0);
            }
            else if ((Mathf.Abs(players[i].transform.position.x - originalPosition.x) > openDistance || Mathf.Abs(players[i].transform.position.y - originalPosition.y) > openDistance) && transform.position.y >= originalPosition.y)
            {
                transform.Translate(0, -.1f, 0);
            }
        }
    }
}

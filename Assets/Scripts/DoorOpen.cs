using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    private GameObject[] players;                   //Array of players
    private int numPlayers;                         //Number of players in the game
    public float playerOpenDistance;                //How close the player needs to be to open the door
    private Vector3 originalPosition;               //Doors original position
    private Vector3 openedPosition;                 //Door max opened position
    public float openDistance;                      //How far the door needs to move to open completely
    public float speed;                             //Door opening speed

    // Use this for initialization
    void Start()
    {
        originalPosition = transform.position;
        openedPosition = new Vector3(transform.position.x,(transform.position.y + openDistance), transform.position.z);
        numPlayers = 1;                                 //Change for multiplayer
        for (int i = 0; i < numPlayers; i++)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numPlayers; i++)        //Open door when player is nearby, else close it
        {
            if (Mathf.Abs(players[i].transform.position.x - originalPosition.x) < playerOpenDistance && Mathf.Abs(players[i].transform.position.y - originalPosition.y) < playerOpenDistance && transform.position.y  <= openedPosition.y)
            {
                transform.Translate(0, speed, 0);
            }
            else if ((Mathf.Abs(players[i].transform.position.x - originalPosition.x) > playerOpenDistance || Mathf.Abs(players[i].transform.position.y - originalPosition.y) > playerOpenDistance) && transform.position.y >= originalPosition.y)
            {
                transform.Translate(0, -speed, 0);
            }
        }
    }
}

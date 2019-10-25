using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedScript : Item {
    private GameObject player;
    private PlayerMovement2 playerMovement;
    private bool speedIncreased = false; //Should only increase speed once

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement2>();
        }
       

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UseItem()
    {
        if (!speedIncreased)
        {
            speedIncreased = !speedIncreased;
            playerMovement.moveSpeed *= 1.5f;
        }
    }
}

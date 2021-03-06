﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnifeScript : Item {

    private ElementalEffects elementScript;
    private GameObject playerWeapon;
    private GameObject player;

	// Use this for initialization
	void Start () //Get player attack object and element attached to it
    { 
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (Transform child in player.transform)
        {
            if(child.tag == "Player Attack")
            {
                playerWeapon = child.gameObject;
                elementScript = child.GetComponent<ElementalEffects>();
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

	}

    public override void UseItem() //Set player basic attack to fire
    {
        if (elementScript != null)
        {
            elementScript.element = elementScript.element | FIRE;
            elementScript.elementChangedBy = gameObject;
            print("AYAYA");
        }
    }


}

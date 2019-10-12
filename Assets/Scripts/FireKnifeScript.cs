using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnifeScript : Item {

    private ElementalEffects elementScript;
    private GameObject playerWeapon;
    private GameObject player;

	// Use this for initialization
	void Start () {
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
        if(type == PASSIVE)
        {
            UseItem();
        }
	}

    public new void UseItem()
    {
        if (elementScript != null)
        {
            elementScript.element = elementScript.element | FIRE;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBody : MonoBehaviour {

    private ElementalEffects playerElementScript;
    private ElementalEffects elementalScript;
    private GameObject player;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerElementScript = player.GetComponent<ElementalEffects>(); //Pick up the element of the player
        elementalScript = GetComponent<ElementalEffects>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playerElementScript != null && elementalScript != null)
        {
            elementalScript.element = playerElementScript.element;
            elementalScript.elementChangedBy = player;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalArmband : MonoBehaviour {

    private ElementalEffects elementScript;
    private ElementalEffects attackElementScript;

	// Use this for initialization
	void Start () {
        elementScript = gameObject.GetComponent<ElementalEffects>();
        attackElementScript = transform.parent.GetComponentInChildren<ElementalEffects>(); //Pick up the element of the first child on the Player object, this should be the atk hitbox
	}
	
	// Update is called once per frame
	void Update () {
        if (elementScript && attackElementScript)
        {
            elementScript.element = attackElementScript.element;
        }
	}
}

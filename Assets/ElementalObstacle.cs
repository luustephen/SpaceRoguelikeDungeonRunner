using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalObstacle : MonoBehaviour {

    private ElementalEffects elementScript;
    public int destroyElement; //Element that will destroy this object

	// Use this for initialization
	void Start () {
        elementScript = gameObject.GetComponent<ElementalEffects>();
    }
	
	// Update is called once per frame
	void Update () {

        if (elementScript)
        {
            if((elementScript.element & destroyElement) == destroyElement) //If the destroy element has been reached
            {
                Destroy(gameObject);
            }
        }
	}

}

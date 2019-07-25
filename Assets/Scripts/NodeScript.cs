using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    public bool occupied = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node")
        {
            occupied = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node")
        {
            occupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Floor" && collision.gameObject.tag != "Player Attack" && collision.gameObject.tag != "Node")
        { 
            occupied = false;
        }
    }
}

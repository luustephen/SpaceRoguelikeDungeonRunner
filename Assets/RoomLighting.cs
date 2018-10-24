using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLighting : MonoBehaviour {

    private GameObject player;
    private Light roomLight;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        roomLight = GetComponent<Light>();

    }
	
	// Update is called once per frame
	void Update () {
		if(player.transform.position.x > -3 && player.transform.position.x < 5 && player.transform.position.y > -3 && player.transform.position.y < 5)
        {
            roomLight.enabled = true;
        }
        else
        {
            roomLight.enabled = false;
        }
	}
}

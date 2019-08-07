using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    private GameObject[] rooms;
    private MapBuilder mapScript;
    private GameObject player;
    private bool firstpass = true;
    private bool focusRoom = false; //if camera is focused on room
    private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
        mapScript = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
        player = GameObject.FindGameObjectWithTag("Player");
        originalPosition = transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (firstpass)
        {
            rooms = mapScript.getAllRooms();
            firstpass = false;
        }
        transform.parent = player.transform;
        transform.localPosition = originalPosition;
        focusRoom = false;
        for(int i = 0; i<rooms.Length; i++)
        {
            if (rooms[i] != null && rooms[i].GetComponent<Room>() != null && rooms[i].GetComponent<Room>().active)
            {
                transform.parent = null;
                transform.position = new Vector3(rooms[i].transform.position.x + 10, rooms[i].transform.position.y, rooms[i].transform.position.z + originalPosition.z);
                focusRoom = true;
                print("room");
            }
        }
    }
}

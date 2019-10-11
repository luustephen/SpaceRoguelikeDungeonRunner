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
    public float speed = 5;

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
        focusRoom = false;
        for(int i = 0; i<rooms.Length; i++)
        {
            if (rooms[i] != null && rooms[i].GetComponent<Room>() != null && rooms[i].GetComponent<Room>().active) //if room is active then put the camera over the room
            {
                transform.parent = null;
                Vector3 roomPosition = new Vector3(rooms[i].transform.position.x + 10, rooms[i].transform.position.y, rooms[i].transform.position.z + originalPosition.z);
                if (Mathf.Abs(transform.position.x - roomPosition.x) > .5f || Mathf.Abs(transform.position.y - roomPosition.y) > .5f)
                {
                    Vector3 normalizedDirection = (roomPosition - transform.position).normalized;
                    transform.Translate(normalizedDirection * speed);
                }
                else if (Mathf.Abs(transform.position.x - roomPosition.x) < .5f && Mathf.Abs(transform.position.y - roomPosition.y) < .5f)
                {
                    transform.position = new Vector3(rooms[i].transform.position.x + 10, rooms[i].transform.position.y, rooms[i].transform.position.z + originalPosition.z);
                }
                focusRoom = true;
            }
        }
        if (!focusRoom)
        {
            transform.parent = player.transform;
            transform.localPosition = originalPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour
{

    private MapBuilder mapBuilder;
    private Vector3 endingRoomPosition;

    // Start is called before the first frame update
    void Start()
    {
        mapBuilder = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
        endingRoomPosition = mapBuilder.GetEndingRoom().transform.position;
        transform.Translate(endingRoomPosition-transform.position);
        transform.Translate(new Vector3(mapBuilder.dimensions/2,0,0));//Put it into the last room in the middle
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
            SceneManager.LoadScene("Planet Surface"); //Next level
    }
}

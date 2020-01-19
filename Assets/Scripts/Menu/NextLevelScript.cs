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
    }

    // Update is called once per frame
    void Update()
    {
        SceneManager.LoadScene("Planet Surface");
    }

}
